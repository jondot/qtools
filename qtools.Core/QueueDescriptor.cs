namespace qtools.Core
{
    public class QueueDescriptor
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                Name = Path.Substring(Path.IndexOf(':', Path.IndexOf('=')) + 1);
            }
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}
