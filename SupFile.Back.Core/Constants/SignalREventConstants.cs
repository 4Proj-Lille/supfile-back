// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SupFile.Back.Core.Constants;

public static class SignalREventConstants
{
    public const string OnUserConnected = "OnUserConnected";
    public const string OnUserDisconnected = "OnUserDisconnected";
    public const string OnUserJoinedChannel = "OnUserJoinedChannel";
    public const string OnUserLeftChannel = "OnUserLeftChannel";
    public const string OnMessageReceived = "OnMessageReceived";
    public const string OnMessageUpdated = "OnMessageUpdated";
    public const string OnMessageDeleted = "OnMessageDeleted";
    public const string OnNotificationReceived = "OnNotificationReceived";
    public const string OnReactionAdded = "OnReactionAdded";
    public const string OnReactionDeleted = "OnReactionDeleted";
}
