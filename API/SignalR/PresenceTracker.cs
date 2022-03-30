namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionID) 
        {
            //we lock the dictionary because we don't want multiple users update this at the same time
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username)) 
                {
                    OnlineUsers[username].Add(connectionID);
                }
                else 
                {
                    OnlineUsers.Add(username, new List<string>{connectionID});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionID) 
        {
            //we lock the dictionary because we don't want multiple users update this at the same time
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;
                
                OnlineUsers[username].Remove(connectionID);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers) 
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}