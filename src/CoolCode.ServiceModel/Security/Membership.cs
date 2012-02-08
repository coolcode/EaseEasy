using System.Web.Security;

namespace CoolCode.ServiceModel.Security {
	public interface IMembership {
		int MinPasswordLength { get; }

		bool ValidateUser(string userName, string password);
		MembershipCreateStatus CreateUser(string userName, string password, string email);
		bool ChangePassword(string userName, string oldPassword, string newPassword);
	}

	public class DefaultMembership : IMembership {
		private MembershipProvider _provider;

		public DefaultMembership()
			: this(null) {
		}

		private DefaultMembership(MembershipProvider provider) {
			_provider = provider ?? Membership.Provider;
		}

		public int MinPasswordLength {
			get {
				return _provider.MinRequiredPasswordLength;
			}
		}

		public bool ValidateUser(string userName, string password) {
			return _provider.ValidateUser(userName, password);
		}

		public MembershipCreateStatus CreateUser(string userName, string password, string email) {
			MembershipCreateStatus status;
			_provider.CreateUser(userName, password, email, null, null, true, null, out status);
			return status;
		}

		public bool ChangePassword(string userName, string oldPassword, string newPassword) {
			MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
			return currentUser.ChangePassword(oldPassword, newPassword);
		}
	}
}
