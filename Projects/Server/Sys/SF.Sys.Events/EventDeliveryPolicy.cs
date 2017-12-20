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


namespace SF.Sys.Events
{
	public enum EventDeliveryPolicy
	{
		/// <summary>
		/// 无保证, 性能最好
		/// </summary>
		NoGuarantee,
		/// <summary>
		/// 尽力保证，尝试尽力投递事件，但不保证成功
		/// </summary>
		TryBest,
		/// <summary>
		/// 最多一次，通过记录事件ID，确保相同事件最多只会投递一次
		/// </summary>
		AtMostOnce,
		/// <summary>
		/// 最少一次，先存储事件，当错误发生时，会重新投递事件
		/// </summary>
		AtLeastOnce,
		/// <summary>
		/// 刚好一次，组合最多一次和最少一次
		/// </summary>
		JustOnce
	}
   
}
