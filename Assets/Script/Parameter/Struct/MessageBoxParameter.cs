using Script.Manager.TableMgr;

namespace Script.Parameter.Struct
{
    public readonly struct MessageBoxParameter
    {
        public readonly string Title;
        public readonly string Desc;
        public readonly string Ok;
        public readonly string Cancel;

        public MessageBoxParameter(string title, string desc, string yes, string cancel) : this()
        {
            Title = GetKeyOrFallback(title, "Notice");
            Desc = GetKeyOrFallback(desc, "!!- ERROR No Desc String");
            Ok = GetKeyOrFallback(yes, "Yes");
            Cancel = GetKeyOrFallback(cancel, "No");
        }       
        
        public MessageBoxParameter(string desc) : this()
        {
            Title = TableManager.Instance.GetTableString("Notice");
            Desc = GetKeyOrFallback(desc, "!!- ERROR No Desc String");
            Ok = TableManager.Instance.GetTableString("Yes");
            Cancel = TableManager.Instance.GetTableString("No");
        }

        private string GetKeyOrFallback(string key, string fallback) =>
            string.IsNullOrEmpty(key) ? TableManager.Instance.GetTableString(fallback) : key;

    }
}