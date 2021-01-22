using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ImageClassification.API.Hubs
{
    public class TrainLogHub : Hub
    {
        public async Task SendMessage(string method, string specification, params object[] arguments)
        {
            await Clients.All.SendAsync(method, specification, arguments);
        }
    }
}
