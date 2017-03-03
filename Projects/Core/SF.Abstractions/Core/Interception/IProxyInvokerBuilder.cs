// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SF.Core.Interception
{
	public interface IProxyInvokerFactory
	{
		IInterceptionBehavior Create(Type InterfaceType,object Target);
	}
}
