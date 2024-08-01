namespace As.Zavrsni.Web.Components.Helpers
{
    public class NavParentItem
    {
        public string NodeId;
        public string NodeText;
        // public string Icon;
        public bool Expanded;
        public bool HasChild => Child.Any();
        public List<NavChildItem> Child;

    }
    public class NavChildItem
    {
        public string NodeId;
        public string NodeText;
        public string ParentId { get; set; }
        public string Url { get; set; }
        // public string Icon;
        public bool Selected;
    }
    public class NavItem
    {
        public int NodeId { get; set; }
        public int? ParentNodeId { get; set; }
        public string NodeText { get; set; }
        public bool Expanded { get; set; }
        public string Url { get; set; }
        public bool HasChild { get; set; }
        public bool Selected { get; set; }
    }
}
