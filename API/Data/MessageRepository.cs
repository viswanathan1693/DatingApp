using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private DataContext _context;
        private IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await  _context.Groups
            .Include(c => c.Connections)
            .Where(c=> c.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                        .Include(u => u.Sender)
                        .Include(u => u.Recipient)
                        .SingleOrDefaultAsync(x => x.Id== id);
        }

        public async Task<PagedList<MessageDTO>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages
            .OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
            .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ =>  query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };

           return await PagedList<MessageDTO>.CreateAsync(query, messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receipientUsername)
        {
            var messages = await _context.Messages
                    .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                     && m.Sender.UserName == receipientUsername
                     || m.Recipient.UserName == receipientUsername
                     && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                    )
                    .OrderBy(m=> m.MessageSent)
                    .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
            && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }                
            }

            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}