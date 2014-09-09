using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Linq;
using System.Text.RegularExpressions;
using qtools.Core.Extensions;

namespace qtools.Core
{
    public interface IQueueTools
    {
        bool Exists(string queue);
        void Delete(string queue);
        void Create(string queue, string user, string permissions, bool transactional, int limit);
        IEnumerable<QueueDescriptor> GetPrivateQueues(string machine, QueueTransaction xactionality);
        IEnumerable<QueueDescriptor> GetPublicQueuesByMachine(string machine, QueueTransaction xactionality);
        void DeleteAllMessages(string queue);
        IEnumerable<MessageDescriptor> Tail(string queue);
        IEnumerable<GrepResult> Grep(string queue, string expression, bool caseInsensitive);
        IEnumerable<CatResult> Cat(string queue, bool withExtension);
        int Count(string queue);
        int Transfer(string subject, string destination, string expression, bool caseInsensitive, bool removeAfter);
    }

    public enum QueueTransaction
    {
        Ignore,
        Transactional,
        NonTransactional
    }

    public class GrepResult
    {
        public MessageDescriptor Message { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}:\t{3}", Message.CreatedAt, Message.Id, Message.Label, Text);
        }
    }

    public class CatResult
    {
        public MessageDescriptor Message { get; set; }

        public string Text { get; set; }
        public byte[] Extension { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}{5}", Message.CreatedAt, Message.Id, Message.Label, Text,
                Extension != null ? System.Text.Encoding.UTF8.GetString(Extension) : "", Extension != null ? "\n" : "");
        }
    }

    public class QueueTools : IQueueTools
    {
        public IEnumerable<MessageDescriptor> Tail(string queue)
        {
            var messagePropertyFilter = new MessagePropertyFilter
                                            {
                                                ArrivedTime = true,
                                                Body = true,
                                                Id = true,
                                                Label = true
                                            };


            var messageQueue = new MessageQueue(queue);
            messageQueue.MessageReadPropertyFilter = messagePropertyFilter;

            var messageEnumerator2 = messageQueue.GetMessageEnumerator2();
            while(true)
            {
                //arbitrary, 1 day.
                while(messageEnumerator2.MoveNext(TimeSpan.FromDays(1)))
                {
                    if(messageEnumerator2.Current == null)
                        continue;

                    yield return CreateMessageDescriptor(messageEnumerator2.Current);
                }
            }
        }

        public IEnumerable<GrepResult> Grep(string subject, string expression, bool caseInsensitive)
        {
            Regex matcher = new Regex("(" + expression + ")", RegexOptions.Compiled | (caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None));
            var messagePropertyFilter = new MessagePropertyFilter
                                            {
                                                ArrivedTime = true,
                                                Body = true,
                                                Id = true,
                                                Label = true
                                            };


            var messageQueue = new MessageQueue(subject);
            messageQueue.MessageReadPropertyFilter = messagePropertyFilter;

            var messageEnumerator2 = messageQueue.GetMessageEnumerator2();
            while (messageEnumerator2.MoveNext())
            {
                if (messageEnumerator2.Current == null)
                    continue;

                var message = messageEnumerator2.Current;
                using (var streamReader = new StreamReader(message.BodyStream))
                {
                    string body = streamReader.ReadToEnd();
                    if (!matcher.IsMatch(body))
                        continue;

                    yield return
                        new GrepResult
                            {Message = CreateMessageDescriptor(message), Text = matcher.HighlightMatch(body, "<{0}>")};
                }
            }
        }

        public IEnumerable<CatResult> Cat(string subject, bool withExtension)
        {
            var messagePropertyFilter = new MessagePropertyFilter
            {
                ArrivedTime = true,
                Body = true,
                Id = true,
                Label = true,
                Extension = withExtension
            };

            var messageQueue = new MessageQueue(subject);
            messageQueue.MessageReadPropertyFilter = messagePropertyFilter;

            var messageEnumerator2 = messageQueue.GetMessageEnumerator2();
            while (messageEnumerator2.MoveNext())
            {
                if (messageEnumerator2.Current == null)
                    continue;

                var message = messageEnumerator2.Current;

                using (var streamReader = new StreamReader(message.BodyStream))
                {
                    string body = streamReader.ReadToEnd();

                    yield return
                        new CatResult { Message = CreateMessageDescriptor(message), Text = body, Extension = withExtension ? message.Extension : null };
                }
            }
        }


        private void Send(Message message, MessageQueue destination, MessageQueueTransaction transaction)
        {
            if (transaction != null)
                destination.Send(message, message.Label, transaction);
            else
                destination.Send(message, message.Label);
        }

        public int Transfer(string subject, string destination, string expression, bool caseInsensitive, bool removeAfter)
        {
            MessageQueueTransaction transaction = null;

            int ret = 0;
            Regex matcher = null;
            
            if (expression != null)
                matcher = new Regex("(" + expression + ")", RegexOptions.Compiled | (caseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None));

            var messagePropertyFilter = new MessagePropertyFilter
            {
                ArrivedTime = true,
                Body = true,
                Id = true,
                Label = true
            };


            var messageQueue = new MessageQueue(subject);
           
            if (!Exists(destination))
            {
                throw new Exception("Can't write to non-existing queue " + destination + ". You can create it with qtouch.");
            }

            var destinationQueue = new MessageQueue(destination);

            if (!destinationQueue.CanWrite)
            {
                throw new Exception("Can't write to non-writable queue " + destination);
            }

            if (destinationQueue.Transactional)
            {
                transaction = new MessageQueueTransaction();
                transaction.Begin();
            }

            destinationQueue.Formatter = messageQueue.Formatter;

            messageQueue.MessageReadPropertyFilter = messagePropertyFilter;

            var messageEnumerator2 = messageQueue.GetMessageEnumerator2();
            while (messageEnumerator2.MoveNext())
            {
                if (messageEnumerator2.Current == null)
                    continue;

                var message = messageEnumerator2.Current;
                if (matcher != null)
                {
                    using (var streamReader = new StreamReader(message.BodyStream))
                    {
                        string body = streamReader.ReadToEnd();

                        if (!matcher.IsMatch(body))
                            continue;

                        Send(message, destinationQueue, transaction);
                    }
                }
                else
                {
                    Send(message, destinationQueue, transaction);
                }

                ret++;

                if (removeAfter)
                    messageEnumerator2.RemoveCurrent();

            }

            if (transaction!=null)
                transaction.Commit();

            return ret;
        }

        public int Count(string queue)
        {
            var messagePropertyFilter = new MessagePropertyFilter
                                            {
                                                AdministrationQueue = false,
                                                ArrivedTime = false,
                                                CorrelationId = false,
                                                Priority = false,
                                                ResponseQueue = false,
                                                SentTime = false,
                                                Body = false,
                                                Label = false,
                                                Id = false
                                            };


            var messageQueue = new MessageQueue(queue);
            messageQueue.MessageReadPropertyFilter = messagePropertyFilter;

            return messageQueue.GetAllMessages().Length;
        }

        private MessageDescriptor CreateMessageDescriptor(Message msg)
        {
            using(var streamReader = new StreamReader(msg.BodyStream))
            {
                string body = streamReader.ReadToEnd();
                return new MessageDescriptor{Id = msg.Id, Content = body, Label = msg.Label, CreatedAt = msg.ArrivedTime };
            }
        }

        public bool Exists(string queue)
        {
            return MessageQueue.Exists(queue);
        }

        public void Delete(string name)
        {
            MessageQueue.Delete(name);
        }

        public void Create(string name, string user, string permissions, bool transactional, int limit)
        {
            var messageQueue = MessageQueue.Create(name, transactional);
            if(!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(permissions))
            {
                messageQueue.SetPermissions(user, (MessageQueueAccessRights) Enum.Parse(typeof(MessageQueueAccessRights), permissions,true));
            }
            if(limit > 0)
            {
                messageQueue.MaximumQueueSize = limit;
            }
        }

        public IEnumerable<QueueDescriptor> GetPublicQueuesByMachine(string machine, QueueTransaction xactionality)
        {
            return MessageQueue.GetPublicQueuesByMachine(machine).Where(x=> ByTransactionality(xactionality, x.Transactional) ).Select(x => new QueueDescriptor { Path = x.Path, Transactional = x.Transactional, Limit = x.MaximumQueueSize });
        }

        public void DeleteAllMessages(string subject)
        {
            new MessageQueue(subject).Purge();
        }

        public IEnumerable<QueueDescriptor> GetPrivateQueues(string machine, QueueTransaction xactionality)
        {
            // Get a list of queues with the specified category.
            return MessageQueue.GetPrivateQueuesByMachine(machine).Where(x => ByTransactionality(xactionality, x.Transactional)).Select(x => new QueueDescriptor { Path = x.Path, Transactional = x.Transactional, Limit = x.MaximumQueueSize });
        }

        private static bool ByTransactionality(QueueTransaction xactionality, bool queueTransactional)
        {
            return xactionality == QueueTransaction.Transactional ? queueTransactional : (xactionality == QueueTransaction.NonTransactional ? !queueTransactional : true);
        }
    }

    public class MessageDescriptor
    {
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public string Label { get; set; }
        public string Content { get; set; }
        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy HH:mm:ss}\t{1}\t{2}\t{3}", CreatedAt,Id, Label, Content.Substring(0, Math.Min(Content.Length, 50)));
        }
    }
}
