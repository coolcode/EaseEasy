using System.Web.Security;

namespace CoolCode.ServiceModel.Security {
	public interface IFormsAuthentication {
		void SignIn(string userName, bool createPersistentCookie);
		void SignOut();
	}

	public class DefaultFormsAuthentication : IFormsAuthentication {
		public void SignIn(string userName, bool createPersistentCookie) {
			FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
		}

		public void SignOut() {
			FormsAuthentication.SignOut();
		}
	}
}
