﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels.Sockets;
using JT808.DotNetty.Abstractions;
using JT808.DotNetty.Abstractions.Enums;
using JT808.DotNetty.Core.Interfaces;
using JT808.DotNetty.Core.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JT808.DotNetty.Core.Impls
{
    class JT808DatagramPacketImpl : IJT808DatagramPacket
    {
        private readonly JT808TrafficService jT808TrafficService;
        private readonly IJT808DownlinkPacket jT808DownlinkPacket;
        public JT808DatagramPacketImpl(
            JT808TrafficServiceFactory jT808TrafficServiceFactory,
            IJT808DownlinkPacket jT808DownlinkPacket)
        {
            this.jT808DownlinkPacket = jT808DownlinkPacket;
            this.jT808TrafficService = jT808TrafficServiceFactory.Create(JT808TransportProtocolType.udp);
        }

        public DatagramPacket Create(byte[] message, EndPoint recipient)
        {
            jT808TrafficService.SendSize(message.Length);
            jT808DownlinkPacket.ProcessorAsync(message, JT808TransportProtocolType.udp);
            return new DatagramPacket(Unpooled.WrappedBuffer(message), recipient);
        }
    }
}
