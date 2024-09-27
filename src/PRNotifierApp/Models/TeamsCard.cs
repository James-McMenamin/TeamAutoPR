using System.Collections.Generic;

namespace PRNotifierApp.Models
{
    public class TeamsCard
    {
        public string Type { get; set; } = "AdaptiveCard";
        public string Version { get; set; } = "1.0";
        public required List<CardElement> Body { get; set; }
        public required List<CardAction> Actions { get; set; }
    }

    public class CardElement
    {
        public required string Type { get; set; }
        public required string Text { get; set; }
        public required string Size { get; set; }
        public required string Weight { get; set; }
        public bool Wrap { get; set; }
    }

    public class CardAction
    {
        public required string Type { get; set; }
        public required string Title { get; set; }
        public required string Url { get; set; }
    }
}