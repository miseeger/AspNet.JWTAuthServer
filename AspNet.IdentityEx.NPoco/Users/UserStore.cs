using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.IdentityEx.NPoco.Claims;
using AspNet.IdentityEx.NPoco.Clients;
using AspNet.IdentityEx.NPoco.Roles;
using AspNet.IdentityEx.NPoco.UserClaims;
using AspNet.IdentityEx.NPoco.UserLogins;
using AspNet.IdentityEx.NPoco.UserRoles;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.IdentityEx.NPoco.Users
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity user store interfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>
		where TUser : IdentityUser
    {
        private UserTable<TUser> _userTable;
        private RoleTable _roleTable;
	    private ClaimTable _claimTable;
        private UserRoleTable _userRoleTable;
        private UserClaimTable _userClaimTable;
        private UserLoginTable _userLoginTable;
	    private ClientTable _clientTable;

        public Database NPocoDb { get; private set; }

        public IQueryable<TUser> Users
        {
            get
            {
                return _userTable.GetUsers().AsQueryable();
            }
        }

        public UserStore()
        {
            new UserStore<TUser>(new Database(IdentityConstants.ConnectionName));
        }

        public UserStore(Database nPocoDb)
        {
            NPocoDb = nPocoDb;
            _userTable = new UserTable<TUser>(nPocoDb);
            _roleTable = new RoleTable(nPocoDb);
			_claimTable = new ClaimTable(nPocoDb);
			_clientTable = new ClientTable(nPocoDb);
            _userRoleTable = new UserRoleTable(nPocoDb);
            _userClaimTable = new UserClaimTable(nPocoDb);
            _userLoginTable = new UserLoginTable(nPocoDb);
        }


        // ----- User funcionality --------------------------------------------

        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

	        if (string.IsNullOrEmpty(user.ClientId))
	        {
				throw new NoNullAllowedException(IdentityConstants.ClientId);
	        }

	        if (_clientTable.GetClientById(user.ClientId) == null)
	        {
				throw new NoNullAllowedException(IdentityConstants.Client);
	        }

            _userTable.Insert(user);

            return Task.FromResult<object>(null);
        }


        public Task<TUser> FindByLoginAsync(IdentityUserLogin login)
        {
            var result = _userLoginTable.FindUserByLogin(login) as TUser;

            return Task.FromResult<TUser>(result);
        }


        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException(IdentityConstants.NoUserId);
            }

            var result = _userTable.GetUserById(userId) as TUser;

            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(IdentityConstants.NoUserName);
            }

            var result = _userTable.GetUserByName(userName) as List<TUser>;

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return Task.FromResult<TUser>(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _userTable.Update(user);

            return Task.FromResult<object>(null);
        }


        // ----- User-Claim funcionality --------------------------------------

        /// <summary>
        /// Adds a claim to the user
		/// Different approach here: check if the Claim object exists in the Claim table
		/// and if so: identify by getting its Id and create the claim assigment to the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}
			
			var claimsFound = _claimTable.GetClaims()
				.Where(c => c.Type == claim.Type && c.Value == claim.Value).ToList();

			if (!claimsFound.Any())
	        {
				throw new ArgumentNullException(IdentityConstants.Claim);   
	        }

			var specificClaim = claimsFound.FirstOrDefault(c => c.ClientId == user.ClientId);
			var globalClaim = claimsFound.FirstOrDefault(c => c.ClientId == null);
			var claimToAdd = specificClaim ?? globalClaim;

			_userClaimTable.Insert(claimToAdd.Id, user.Id);

            return Task.FromResult<object>(null);
        }


		/// <summary>
		/// TODO: --> Add to UserManagerEx/Extensions
		/// Adds a client specitic (or global) claim to the user
		/// </summary>
		/// <param name="user"></param>
		/// <param name="claim"></param>
		/// <returns></returns>
		public Task AddClaimAsync(TUser user, IdentityClaim claim)
		{
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			if (claim == null)
			{
				throw new ArgumentNullException(IdentityConstants.Claim);
			}

			_userClaimTable.Insert(claim.Id, user.Id);

			return Task.FromResult<object>(null);
		}


        /// <summary>
        /// Gets the claims of a user as List of Claim
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            var identity = _userClaimTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }


        /// <summary>
		/// TODO: --> Add to UserManager/ExExtensions
        /// Gets the claims of a user as List of IdentitiClaim
        /// which contains ClientIds
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<IList<IdentityClaim>> GetClaimsAsyncEx(TUser user)
        {
            var result = _userClaimTable.GetClaims(user.Id);
            return Task.FromResult<IList<IdentityClaim>>(result);
        }


        /// <summary>
        /// Removes a claim assignment from the user.
		/// New Approach: Check if the given Claim object is assigned to the user and delete the 
		/// assignment(s), if existent. It's not client-agnostic, here!
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
		public Task RemoveClaimAsync(TUser user, Claim claim)
        {
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			if (claim == null)
			{
				throw new ArgumentNullException(IdentityConstants.Claim);
			}

			var claimsAssigned = _userClaimTable
				.GetClaims(user.Id)
				.Where(c => c.Type == claim.Type && c.Value == claim.Value)
				.ToList();

	        foreach (var identityClaim in claimsAssigned)
	        {
				_userClaimTable.Delete(identityClaim, user);
	        }

			return Task.FromResult<object>(null);
        }


        /// <summary>
		/// TODO: --> Add to UserManagerEx/Extensions
        /// Deletes the assigment of a special (or global) claim assigment
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
		public Task RemoveClaimAsync(TUser user, IdentityClaim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            _userClaimTable.Delete(claim, user);

            return Task.FromResult<object>(null);
        }


        // ----- User-Login funcionality --------------------------------------


        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            _userLoginTable.Insert(new IdentityUserLogin()
                {
                    ClientId = user.ClientId
                    , UserId = user.Id
                    , LoginProvider = login.LoginProvider
                    , ProviderKey = login.ProviderKey
                }
            );

            return Task.FromResult<object>(null);
        }


        public Task AddLoginAsync(IdentityUserLogin login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            _userLoginTable.Insert(login);

            return Task.FromResult<object>(null);
        }


        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            throw new NotImplementedException(IdentityConstants.UserLogin);
        }


        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            var userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            var logins = _userLoginTable.FindByUserId(user.Id);

            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }


        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            _userLoginTable.Delete(new IdentityUserLogin()
            {
                ClientId = user.ClientId
                , UserId = user.Id
                , LoginProvider = login.LoginProvider
                , ProviderKey = login.ProviderKey
            });

            return Task.FromResult<object>(null);
        }


        public Task RemoveLoginAsync(IdentityUserLogin login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            _userLoginTable.Delete(login);

            return Task.FromResult<Object>(null);
        }


        // ----- User-Role funcionality ---------------------------------------

		/// <summary>
		/// Add a role by ist name: Check if a global or client specific role exists 
		/// (take user.ClientId to specify) --> assign it ;-) If not: Take a possibly 
		/// existing global role.

		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

	        var rolesFound = _roleTable.GetRoles().Where(r => r.Name == roleName).ToList();

			if (!rolesFound.Any())
	        {
				throw new ArgumentNullException(IdentityConstants.Role);   
	        }

	        var specificRole = rolesFound.FirstOrDefault(r => r.ClientId == user.ClientId);
	        var globalRole = rolesFound.FirstOrDefault(r => r.ClientId == null);
	        var roleToAdd = specificRole ?? globalRole;

			_userRoleTable.Insert(user, roleToAdd.Id);

			return Task.FromResult<object>(null);
        }


		/// <summary>
		/// TODO: --> Add to UserManagerEx/Extensions
		/// Adds an IdentityRole to a user
		/// </summary>
		/// <param name="user"></param>
		/// <param name="role"></param>
		/// <returns></returns>
        public Task AddToRoleAsync(TUser user, IdentityRole role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (role == null)
            {
                throw new ArgumentException(IdentityConstants.Role);
            }

            _userRoleTable.Insert(user, role.Id);

            return Task.FromResult<object>(null);
        }
 

        /// <summary>
        /// Gets the names or the roles attached to a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
		public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            var roles = _userRoleTable.GetRoles(user.Id);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles.Select(r => r.Name).ToList());
                }
            }

            return Task.FromResult<IList<string>>(null);
        }


        /// <summary>
		/// TODO: --> Add to UserManagerEx/Extensions
        /// Gets the roles attachted to the user as IdentityRole.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		public Task<IList<IdentityRole>> GetRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(IdentityConstants.NoUserId);
            }

            var roles = _userRoleTable.GetRoles(userId);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<IdentityRole>>(roles);
                }
            }

            return Task.FromResult<IList<IdentityRole>>(null);
        }


        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            var roles = _userRoleTable.GetRoles(user.Id)
				.Select(r => r.Name)
				.ToList();
            {
                if (roles.Any() && roles.Contains(role))
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }


        public Task RemoveFromRoleAsync(TUser user, string role)
        {
			if (user == null)
			{
				throw new ArgumentNullException(IdentityConstants.User);
			}

			var rolesFound = _roleTable.GetRoles().Where(r => r.Name == role).ToList();

			if (!rolesFound.Any())
			{
				throw new ArgumentNullException(IdentityConstants.Role);
			}

			var specificRole = rolesFound.FirstOrDefault(r => r.ClientId == user.ClientId);
			var globalRole = rolesFound.FirstOrDefault(r => r.ClientId == null);
			var roleToRemove = specificRole ?? globalRole;
			
            return Task.FromResult<int>(_userRoleTable.Delete(roleToRemove.Id, user.Id));
        }


        public Task RemoveFromRoleAsync(TUser user, IdentityRole role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            return Task.FromResult<int>(_userRoleTable.Delete(role.Id, user.Id));
        }


        // ----- Extended User funcionality -----------------------------------

        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                _userTable.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }


        public Task<string> GetPasswordHashAsync(TUser user)
        {
            var passwordHash = _userTable.GetPasswordHash(user.Id);

            return Task.FromResult<string>(passwordHash);
        }


        public Task<bool> HasPasswordAsync(TUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(_userTable.GetPasswordHash(user.Id));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }


        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }


        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }


        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }


        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            _userTable.Update(user);

            return Task.FromResult(0);

        }


        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }


        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }


        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(IdentityConstants.Email);
            }

            var result = _userTable.GetUserByEmail(email) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }


        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }


        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }


        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }


        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            _userTable.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }


        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }


        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }


        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


		public void Dispose()
		{
			if (NPocoDb != null)
			{
				NPocoDb.Dispose();
				NPocoDb = null;
			}

			_userTable = null;
			_roleTable = null;
			_claimTable = null;
			_clientTable = null;
			_userRoleTable = null;
			_userClaimTable = null;
			_userLoginTable = null;
		}

    }

}
