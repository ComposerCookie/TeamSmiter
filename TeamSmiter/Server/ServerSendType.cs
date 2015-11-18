using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public enum ServerSendType
    {
        SendReceivedConnection,
        SendRequestPassword,
        SendApprovedConnection,
        SendRejectedConnection,
        SendNewConnection,
        SendBroadcastChatMessage,
        SendWhiteboardDrawnSomething,
        SendWhiteboardSendBoard,
        SendWhiteboardGivePen,
        SendWhiteboardTakePen,
        SendLoadMap,
        SendTeamUpdated,
        SendGodMoved
    }
}
