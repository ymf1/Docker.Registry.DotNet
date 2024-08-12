//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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


//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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

using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.Catalogs;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class CatalogOperations(RegistryClient client) : ICatalogOperations
{
    public async Task<CatalogResponse> GetCatalog(
        CatalogParameters? parameters = null,
        CancellationToken token = default)
    {
        parameters ??= new CatalogParameters();

        var queryParameters = new QueryString();

        queryParameters.AddFromObject(parameters);

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/_catalog",
            queryParameters,
            token: token);

        return client.JsonSerializer.DeserializeObject<CatalogResponse>(response.Body);
    }
}