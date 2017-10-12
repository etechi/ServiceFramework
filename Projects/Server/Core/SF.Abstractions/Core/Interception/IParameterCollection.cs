#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System.Collections;
using System.Reflection;

namespace SF.Core.Interception
{
	/// <summary>
	/// This interface represents a list of either input or output
	/// parameters. It implements a fixed size list, plus a couple
	/// of other utility methods.
	/// </summary>
	public interface IParameterCollection : IList
    {
        /// <summary>
        /// Fetches a parameter's value by name.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <returns>value of the named parameter.</returns>
        object this[string parameterName] { get; set; }

        /// <summary>
        /// Gets the name of a parameter based on index.
        /// </summary>
        /// <param name="index">Index of parameter to get the name for.</param>
        /// <returns>Name of the requested parameter.</returns>
        string ParameterName(int index);

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by index.
        /// </summary>
        /// <param name="index">Index for this parameter.</param>
        /// <returns>ParameterInfo object describing the parameter.</returns>
        ParameterInfo GetParameterInfo(int index);

        /// <summary>
        /// Gets the ParameterInfo for a particular parameter by name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>ParameterInfo object for the named parameter.</returns>
        ParameterInfo GetParameterInfo(string parameterName);

        /// <summary>
        /// Does this collection contain a parameter value with the given name?
        /// </summary>
        /// <param name="parameterName">Name of parameter to find.</param>
        /// <returns>True if the parameter name is in the collection, false if not.</returns>
        bool ContainsParameter(string parameterName);
    }
}
