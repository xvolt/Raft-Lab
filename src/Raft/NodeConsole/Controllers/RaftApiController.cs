using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RaftCore;
using RaftCore.Components;
using RaftCore.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodeConsole.Controllers
{
    public class RaftApiController:ControllerBase
    {
        public RaftApiController(RaftNode node)
        {
            Node = node;
        }

        public RaftNode Node { get; }

        [HttpPost]
        public Result<bool> RequestVote(int term, uint candidateId, int lastLogIndex, int lastLogTerm)
        {
            return Node.RequestVote(term, candidateId, lastLogIndex, lastLogIndex);
        }

        [HttpPost]
        public Result<bool> AppendEntries([FromBody]AppendEntriesDto dto)
        {
            dto.entries = dto.entries != null && dto.entries.Count == 0 ? null : dto.entries;
            if (dto.entries != null) Console.WriteLine($"entries count={dto.entries.Count}");
            var result = Node.AppendEntries(dto.term, dto.leaderId, dto.prevLogIndex, dto.prevLogTerm, dto.entries, dto.leaderCommit);
            return result;
        }

        [HttpPost]
        public void MakeRequest(string command)
        {
            Node.MakeRequest(command);
        }

        [HttpGet]
        public string GetState(string param=null)
        {
            return Node.StateMachine.RequestStatus(param);
        }
    }
}
