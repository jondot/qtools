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
        IEnumerable<QueueDescriptor> GetPrivateQueues(string machine);
        IEnumerable<QueueDescriptor> GetPublicQueuesByMachine(string machine);
        void DeleteAllMessages(string queue);
        IEnumerable<MessageDescriptor> Tail(string queue);
        IEnumerable<GrepResult> Grep(string queue, string expression, bool caseInsensitive);
        int Count(string queue);
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

        public IEnumerable<QueueDescriptor> GetPublicQueuesByMachine(string machine)
        {
            return MessageQueue.GetPublicQueuesByMachine(machine).Select(x => new QueueDescriptor { Path = x.Path });
        }

        public void DeleteAllMessages(string subject)
        {
            new MessageQueue(subject).Purge();
        }

        public IEnumerable<QueueDescriptor> GetPrivateQueues(string machine)
        {
            // Get a list of queues with the specified category.
            return MessageQueue.GetPrivateQueuesByMachine(machine).Select(x => new QueueDescriptor { Path = x.Path });
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
