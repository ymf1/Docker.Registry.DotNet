// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Net;
global using System.Net.Http;
global using System.Net.Http.Headers;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using Docker.Registry.DotNet.Authentication;
global using Docker.Registry.DotNet.Endpoints;
global using Docker.Registry.DotNet.Helpers;
global using Docker.Registry.DotNet.Models;
global using Docker.Registry.DotNet.OAuth;
global using Docker.Registry.DotNet.QueryParameters;
global using Docker.Registry.DotNet.Registry;

global using JetBrains.Annotations;

global using Newtonsoft.Json;