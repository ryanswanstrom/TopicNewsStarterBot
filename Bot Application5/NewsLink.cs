using System;

namespace Bot_Application5
{
    public class NewsLink
    {
        public NewsLink(string url, string title)
        {
            this.Url = new Uri(url);
            this.Title = title;
        }
        public string Title { get; private set; }
        public Uri Url { get; private set; }
    }
}