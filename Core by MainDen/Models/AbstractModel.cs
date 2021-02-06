using Core_by_MainDen.System;

namespace Core_by_MainDen.Models
{
    public abstract class AbstractModel
    {
        private readonly Access access = new Access();

        public Access Access
        {
            get
            {
                return access;
            }
        }
    }
}
