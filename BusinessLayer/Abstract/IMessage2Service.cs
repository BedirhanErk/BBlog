﻿using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface IMessage2Service : IGenericService<Message2>
    {
        List<Message2> GetInboxListByWriter(int id);
        List<Message2> GetInboxListByWriterLastThreeAndUnread(int id);
        Message2 GetMessageById(int id);
        string GetInboxUnReadMessageCount(int id);
        List<Message2> GetSendboxListByWriter(int id);
    }
}
