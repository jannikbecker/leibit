
namespace Leibit.Tests.Comparer
{
    internal class Identifier
    {
        private object[] m_Members;

        internal Identifier(params object[] members)
        {
            m_Members = members;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Identifier))
                return false;

            var identifier = obj as Identifier;

            if (identifier.m_Members.Length != this.m_Members.Length)
                return false;

            for (int i = 0; i < m_Members.Length; i++)
            {
                if (!m_Members[i].Equals(identifier.m_Members[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
