using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChatCli.Extensions;
using ChatService.Interfaces;
using ChatService.Models;

namespace ChatCli.Interactions
{
    internal class ChatBotInteractions
    {
        private static readonly string[] Jokes =
        {
            @"Algorithm... Word used by programmers when they don't want to explain what they did.",
            @"""Knock, knock."" ... ""Who’s there?"" ... very long pause... ""Java.""",
            @"Its not a bug, its a feature.",
            @"Q: how many programmers does it take to change a light bulb? ... A: none, that's a hardware problem",
            @"Q: ""Whats the object-oriented way to become wealthy?"" ...  A: Inheritance",
            @" [""hip"",""hip""] ... (hip hip array!)"
        };

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);


        public static void RunEventLoop(IChatService serviceClient)
        {
            var lastMessageTimestamp = DateTime.UtcNow;
            while (true)
            {
                foreach (var message in GetAllNewMessages(serviceClient, lastMessageTimestamp))
                {
                    if (IsAJokeRequest(message)) TellAJoke(serviceClient, message);

                    lastMessageTimestamp = message.TimeStamp;
                }

                WaitBeforeNextRun(10);
            }
        }

        private static IEnumerable<ChatMessage> GetAllNewMessages(IChatService serviceClient,
            DateTime lastMessageTimestamp)
        {
            return ClientConsole.GetAllMessages(serviceClient)
                .SkipWhile(message => message.TimeStamp <= lastMessageTimestamp);
        }

        private static bool IsAJokeRequest(ChatMessage message)
        {
            return message.Message.CaseInsensitiveContains("joke");
        }

        private static void TellAJoke(IChatService serviceClient, ChatMessage message)
        {
            PrintReceivedMessage(message);
            var messageToSend = GetRandomJoke();
            SendMessage(serviceClient, messageToSend);
        }

        private static string GetRandomJoke()
        {
            return Jokes[Random.Next(Jokes.Length)];
        }

        private static void SendMessage(IChatService serviceClient, string sendMessage)
        {
            PrintSendMessage(sendMessage);
            ClientConsole.SendMessage(serviceClient, sendMessage);
        }

        private static void PrintSendMessage(string sendMessage)
        {
            Console.WriteLine($"Sending message> {sendMessage}");
        }

        private static void PrintReceivedMessage(ChatMessage message)
        {
            Console.WriteLine($"Received message> {message.Message}");
        }

        private static void WaitBeforeNextRun(int seconds)
        {
            Thread.Sleep(new TimeSpan(0, 0, seconds));
        }
    }
}