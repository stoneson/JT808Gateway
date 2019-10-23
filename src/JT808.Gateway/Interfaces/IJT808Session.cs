﻿using DotNetty.Transport.Channels;
using JT808.Gateway.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Interfaces
{
    public interface IJT808Session
    {
        /// <summary>
        /// 终端手机号
        /// </summary>
        string TerminalPhoneNo { get; set; }
        IChannel Channel { get; set; }
        DateTime LastActiveTime { get; set; }
        DateTime StartTime { get; set; }
        JT808TransportProtocolType TransportProtocolType { get; set; }
    }
}
