
using System.Threading.Tasks;
using System.Data.Common;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Annotations;

namespace SF.Common.UserGroups.Front
{
	
	/// <summary>
	/// 用户组服务
	/// </summary>
	[NetworkService]
	public interface IUserGroupService<TGroup,TMember>
		where TGroup:Group
		where TMember:GroupMember
	{
		/// <summary>
		/// 查询组成员
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<TMember>> QueryMembers(MemberQueryArgument Arg);

		/// <summary>
		/// 查询用户组
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<TGroup>> QueryGroups(GroupQueryArgument Arg);
		
		
		/// <summary>
		/// 将某人从自己的用户组中移除
		/// </summary>
		/// <param name="MemberId">成员ID</param>
		/// <returns></returns>
		Task RemoveMember(long MemberId);

		/// <summary>
		/// 离开某个已加入的用户组
		/// </summary>
		/// <param name="SessionId">用户组ID</param>
		/// <returns></returns>
		Task LeaveGroup(long SessionId);

		/// <summary>
		/// 修改成员信息
		/// </summary>
		/// <param name="Member">成员信息</param>
		/// <returns></returns>
		Task UpdateMember(TMember Member);

		/// <summary>
		/// 修改组信息
		/// </summary>
		/// <param name="Group">用户组信息</param>
		/// <returns></returns>
		Task UpdateGroup(TGroup Group);


		/// <summary>
		/// 设置成员加入用户组状态
		/// </summary>
		/// <remarks>若当前用户是用户组所有者,则设置所有者一方假如状态,否则设置成员方加入状态</remarks>
		/// <param name="MemberId">用户组成员ID</param>
		/// <param name="AcceptType">是否愿意加入</param>
		/// <returns></returns>
		Task SetAcceptType(long MemberId,bool AcceptType);

	}
}