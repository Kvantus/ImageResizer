using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace ImageResizer
{
    /// <summary>
    /// Класс, содержащий информацию о дозволенных группах пользователей и проверяющий текуего пользователя
    /// </summary>
    class UserVerificator
    {
        public string[] FriendlyGroups { get; }

        /// <summary>
        /// Инициализация строкового массива дозволенных групп пользователей заданным массивом
        /// </summary>
        /// <param name="myGroups"></param>
        public UserVerificator(params string[] myGroups)
        {
            if (myGroups.Length == 0)
            {
                throw new ArgumentException("Должна быть объявлена хотя бы одна дозволенная группа пользователей");
            }
            FriendlyGroups = myGroups;
        }

        /// <summary>
        /// Метод проверяет, принадлежит ли текущий пользователь дозволенным группам пользователей
        /// </summary>
        /// <returns>Возсращает true, если пользователь принадлежит одной из дозволенных групп, false в противном случае</returns>
        public bool IsUserValid()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                // поиск текущего пользователя
                UserPrincipal myUser = UserPrincipal.FindByIdentity(context, currentUser.Name);

                // получение массива групп пользователя
                var currentUserGroups = myUser.GetAuthorizationGroups();

                var join = from uGroup in currentUserGroups
                           join fGroup in FriendlyGroups
                           on uGroup.Name equals fGroup
                           select uGroup.Name;

                if (join.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
