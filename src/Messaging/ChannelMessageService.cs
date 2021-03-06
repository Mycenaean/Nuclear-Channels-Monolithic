﻿// Copyright © Nikola Milinkovic 
// Licensed under the MIT License (MIT).
// See License.md in the repository root for more information.

using Newtonsoft.Json;
using Nuclear.Channels.Authentication;
using Nuclear.Channels.Base.Enums;
using Nuclear.Channels.Data.Logging;
using Nuclear.ExportLocator.Decorators;
using Nuclear.ExportLocator.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Nuclear.Channels.UnitTests")]
namespace Nuclear.Channels.Messaging
{
    [Export(typeof(IChannelMessageService), Lifetime = ExportLifetime.Transient)]
    internal class ChannelMessageService : IChannelMessageService
    {
        public void WriteHttpResponse(object chResponse, HttpListenerResponse response)
        {
            response.ContentType = "application/json";
            Stream stream = response.OutputStream;
            using (StreamWriter writer = new StreamWriter(stream))
            {
                try
                {
                    IChannelMessage respChMessage;
                    if (chResponse == null)
                        respChMessage = new ChannelMessage() { Message = "ChannelMethod executed" };
                    else if (chResponse.GetType() != typeof(IChannelMessage) && chResponse.GetType() != typeof(ChannelMessage))
                    {
                        respChMessage = new ChannelMessage
                        {
                            Success = true,
                            Output = chResponse
                        };
                    }
                    else
                        respChMessage = null;

                    //Serialization Logic
                    string outputMsg = string.Empty;
                    JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                    jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    if (respChMessage == null)
                        outputMsg = JsonConvert.SerializeObject(chResponse, Formatting.Indented, jsonSettings);
                    else
                        outputMsg = JsonConvert.SerializeObject(respChMessage, Formatting.Indented, jsonSettings);

                    writer.WriteLine(outputMsg);
                }
                catch (Exception ex)
                {
                    ExceptionHandler(writer, ex, response);
                }
            }
        }

        public void WriteHttpResponseAsPureJson(object chResponse, HttpListenerResponse response, bool serialized)
        {
            response.ContentType = "application/json";
            Stream stream = response.OutputStream;
            using (StreamWriter writer = new StreamWriter(stream))
            {
                try
                {
                    string outputMsg = string.Empty;

                    if (chResponse.GetType() == typeof(string) && serialized)
                    {
                        outputMsg = (string)chResponse;
                    }
                    else
                    {
                        JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                        jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        if (!serialized && chResponse.GetType() == typeof(string))
                        {
                            //Create data key so that json can be properly displayed
                            Dictionary<string, string> data = new Dictionary<string, string>
                            {
                                { "data", (string)chResponse }
                            };
                            outputMsg = JsonConvert.SerializeObject(data, Formatting.Indented, jsonSettings);
                        }
                        else
                            outputMsg = JsonConvert.SerializeObject(chResponse, Formatting.Indented, jsonSettings);
                    }

                    writer.WriteLine(outputMsg);
                }
                catch (Exception ex)
                {
                    ExceptionHandler(writer, ex, response);
                }
            }
        }

        public void ExceptionHandler(StreamWriter writer, Exception ex, HttpListenerResponse response)
        {
            LogChannel.Write(LogSeverity.Error, "Exception handler called..");
            ChannelMessage errorChMessage = new ChannelMessage()
            {
                Message = ex.Message
            };
            LogChannel.Write(LogSeverity.Error, ex.Message);
            LogChannel.Write(LogSeverity.Error, ex.InnerException == null ? "No Inner Exception" : ex.InnerException.ToString());
            response.ContentType = "application/json";
            string outputMsg = JsonConvert.SerializeObject(errorChMessage, Formatting.Indented);
            writer.Write(outputMsg);
        }

        public void FailedAuthenticationResponse(ChannelAuthenticationSchemes ChannelSchema, HttpListenerResponse response)
        {
            LogChannel.Write(LogSeverity.Info, "Authentication failed...Exiting");
            IChannelMessage msg = new ChannelMessage()
            {
                Success = false,
                Message = $"You need to provide {ChannelSchema.ToString()} authentication"
            };
            string outputString = JsonConvert.SerializeObject(msg, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(outputString);
            }
        }

        public void FailedAuthorizationResponse(HttpListenerResponse response)
        {
            LogChannel.Write(LogSeverity.Info, "Authorization failed...Exiting");
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            IChannelMessage msg = new ChannelMessage()
            {
                Success = false,
                Message = $"Unauthorized"
            };
            string outputString = JsonConvert.SerializeObject(msg, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(outputString);
            }
        }

        public void WrongHttpMethod(HttpListenerResponse response, ChannelHttpMethod HttpMethod)
        {
            IChannelMessage msg = new ChannelMessage()
            {
                Message = $"Wrong HTTP Method used. In order to call this endpoint u need to send {HttpMethod.ToString()} request"
            };
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            LogChannel.Write(LogSeverity.Error, "Wrong HTTP Method used");
            string outputString = JsonConvert.SerializeObject(msg, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(response.OutputStream))
            {
                writer.WriteLine(outputString);
            }
        }
    }
}
