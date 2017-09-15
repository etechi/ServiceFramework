// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SF.Core.Interception
{
	public interface IInterceptingProxyTypeBuilder
	{
		Type Build(Type BaseType, params Type[] InterfaceTypes);
	}
}
