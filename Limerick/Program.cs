using Limerick;

DateTimeOffset? _lastMessage=null;
var _guardianRss=new Rss();
var _openAI = new OpenAIGen();
var _mastodon = new Mastodon();

while (true)
{
    Console.WriteLine($"Getting news since '{_lastMessage}'");

    var messages=await _guardianRss.GetArticlesSince(_lastMessage);
    foreach (var message in messages)
    {
        
        message.Title=await _openAI.GetLimerickFromTitle(message.Title);
        if (message.Title!=null)     await _mastodon.SendArticleToMastodon(message);
    }

    if (messages.Count > 0) _lastMessage = messages.Max(q => q.Date);

    Thread.Sleep(TimeSpan.FromMinutes(5));

}

