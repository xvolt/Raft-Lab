using System;
using System.Collections.Generic;
using RaftCore;
using RaftCore.Connections;
using RaftCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace RaftCore.Connections.Implementations {
    /// <summary>
    /// Connects to nodes through a defined WEB API
    /// </summary>
    public class APIRaftConnector : IRaftConnector {
        /// <summary>
        /// ID matching an existing node's ID.
        /// </summary>
        public uint NodeId { get; private set; }
        private string baseURL { get; set; }

        public bool IsAlive { get; private set; }

        private readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Initializes a connector through a base URL.
        /// </summary>
        /// <param name="nodeId">ID that represents this connector's node</param>
        /// <param name="baseURL">Base URL to make requests on</param>
        public APIRaftConnector(uint nodeId, string baseURL) {
            this.NodeId = nodeId;
            if (!baseURL.EndsWith("/")) {
                baseURL += "/";
            }
            this.baseURL = baseURL;
        }

        /// <summary>
        /// Calls the MakeRequest method on the node.
        /// For this, it makes a POST request to the endpoint baseURL/makerequest
        /// </summary>
        /// <param name="command">String containing the request to send to the node</param>
        public void MakeRequest(string command) {
            // convert params to json
            // make POST <baseurl>/makerequest
            SendMakeRequest(command);
        }

        /// <summary>
        /// Calls the RequestVote method on the node.
        /// For this, it makes a POST request to the endpoint baseURL/requestvote
        /// And interprets the resulting JSON as a result.
        /// </summary>
        /// <param name="term">Term of the candidate</param>
        /// <param name="candidateId">Node ID of the candidate</param>
        /// <param name="lastLogIndex">Index of candidate's last log entry</param>
        /// <param name="lastLogTerm">Term of candidate's last log entry</param>
        /// <returns>Returns a Result object containing the current term of the node and whether it grants the requested vote</returns>
        public Result<bool> RequestVote(int term, uint candidateId, int lastLogIndex, int lastLogTerm) {
            // convert params to json
            // make POST <baseurl>/requestvote
            // parse response into a Result object
            var res = SendRequestVote(term, candidateId, lastLogIndex, lastLogTerm).Result;

            return ParseResultFromJSON(res);
        }

        /// <summary>
        /// Calls the AppendEntries method on the node.
        /// For this, it makes a POST request to the endpoint baseURL/appendentries
        /// And interprets the resulting JSON as a result.
        /// </summary>
        /// <param name="term">Leader's current term number</param>
        /// <param name="leaderId">ID of the node invoking this method</param>
        /// <param name="prevLogIndex">Index of log immediately preceding new ones</param>
        /// <param name="prevLogTerm">Term of prevLogIndex entry</param>
        /// <param name="entries">List of entries sent to be replicated. null for heartbeat</param>
        /// <param name="leaderCommit">Leader's CommitIndex</param>
        /// <returns>Returns a Result object containing the current term of the node and whether the request worked</returns>
        public Result<bool> AppendEntries(int term, uint leaderId, int prevLogIndex, int prevLogTerm, List<LogEntry> entries, int leaderCommit) {
            // convert params to json
            // make POST <baseurl>/appendentries
            // parse response into a Result object
            var res = SendAppendEntries(term, leaderId, prevLogIndex, prevLogTerm, entries, leaderCommit).Result;
            var result = ParseResultFromJSON(res);
            if (result == null)
            {
                return new Result<bool>(false, term);
            }
            return result;
        }

        /// <summary>
        /// Calls the TestConnection method on the node.
        /// For this, it makes a GET request to the endpoint baseURL/test
        /// </summary>
        public void TestConnection() {
            SendTestConnection();
        }

        // **************************************
        // **************************************

        private async void SendMakeRequest(string command) {
            var req = new Dictionary<string, string> { { "request", command } };

            var content = new FormUrlEncodedContent(req);

            var response = await client.PostAsync(baseURL + "makerequest", content);
        }

        private async Task<string> SendRequestVote(int term, uint candidateId, int lastLogIndex, int lastLogTerm) {
            var req = new Dictionary<string, string>
            {
               { "term", term.ToString() },
               { "candidateId", candidateId.ToString() },
               { "lastLogIndex", lastLogIndex.ToString() },
               { "lastLogTerm", lastLogTerm.ToString() }
            };

            var content = new FormUrlEncodedContent(req);

            var response = await client.PostAsync(baseURL + "requestvote", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private async Task<string> SendAppendEntries(int term, uint leaderId, int prevLogIndex, int prevLogTerm, List<LogEntry> entries, int leaderCommit)
        {
            var req = new {
               term ,
               leaderId ,
               prevLogIndex ,
               prevLogTerm ,
               entries ,
               leaderCommit
            };

            var content = new StringContent(JsonConvert.SerializeObject(req),Encoding.Default, "application/json");
            try
            {

                var response = await client.PostAsync(baseURL + "appendentries", content);

                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        //private async Task<string> SendAppendEntries(int term, uint leaderId, int prevLogIndex, int prevLogTerm, List<LogEntry> entries, int leaderCommit)
        //{
        //    var req = new Dictionary<string, object>
        //    {
        //       { "term", term.ToString() },
        //       { "leaderId", leaderId.ToString() },
        //       { "prevLogIndex", prevLogIndex.ToString() },
        //       { "prevLogTerm", prevLogTerm.ToString() },
        //       { "entries", entries },
        //       { "leaderCommit", leaderCommit.ToString() }
        //    };
        //    WebClient webClient = new WebClient();
        //    try
        //    {
        //        webClient.Headers["content-type"] = "application/json";
        //        var reqString = Encoding.Default.GetBytes(JsonConvert.SerializeObject(req, Formatting.Indented));
        //        var resByte = webClient.UploadData(baseURL + "appendentries", "post", reqString);
        //        var resString = Encoding.Default.GetString(resByte);
        //        return resString;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return "";
        //    }
        //    finally
        //    {
        //        webClient.Dispose();
        //    }
        //}

        private async void SendTestConnection() {
            try
            {
                var response = await client.GetAsync(baseURL + "test");
            }
            catch
            {
                IsAlive = false;
            }
        }

        private Result<bool> ParseResultFromJSON(string res) {
            try
            {
                var result = JsonConvert.DeserializeObject<Result<bool>>(res);

                return result;
            }
            catch {
                return null;
            }
        }
    }
}
