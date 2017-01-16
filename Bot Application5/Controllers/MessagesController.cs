using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Bot_Application5
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // collect incoming text
                string message = (activity.Text ?? string.Empty).ToLower();
                string output = "try typing 'news <somenews topic>' ";

                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                if(message.StartsWith("news")) {
                    string topic = message.Substring(4).Trim();
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Here is your daily '{topic}' news");
                    sb.Append(Environment.NewLine);

                    /*var news = GetNews(topic);
                    foreach (var item in news)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append(item.Title);
                        sb.Append(Environment.NewLine);
                        sb.Append($"  {item.Url}");
                        sb.Append(Environment.NewLine);
                    }*/
                    output = sb.ToString();
                }


                // return our reply to the user
                Activity reply = activity.CreateReply(output);// ($"You sent {activity.Text} which was {length} characters. You Rock!!");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private List<NewsLink> GetNews(string topic)
        {
            List<NewsLink> news = new List<NewsLink>(5);

            // call the rest API
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<key goes here>");

            // Request parameters
            queryString["q"] = topic;
            queryString["count"] = "5";
            queryString["offset"] = "0";
            queryString["freshness"] = "Day";
            queryString["mkt"] = "en-us";
            queryString["safeSearch"] = "Strict";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?" + queryString;

            var response = client.GetAsync(uri).Result;
            var contents = response.Content.ReadAsStringAsync().Result;

            dynamic json = JsonConvert.DeserializeObject(contents);
            dynamic values = json.value;
            foreach (var v in values)
            {
                var lnk = new NewsLink((string)v.url, (string)v.name);
                news.Add(lnk);
            }
            
            return news;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}