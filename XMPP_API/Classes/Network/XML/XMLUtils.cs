using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace XMPP_API.Classes.Network.XML
{
    static class XMLUtils
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        /// <summary>
        /// Tries to find the given node by its name in the given XmlNode.
        /// Returns null, if nothing found.
        /// </summary>
        /// <param name="node">The node, containing the node.</param>
        /// <param name="name">The node name.</param>
        /// <returns>Returns null if node does not exist, else the node.</returns>
        public static XmlNode getChildNode(XmlNode node, in string name)
        {
            return getChildNode(node, name, null, (string)null);
        }

        /// <summary>
        /// Tries to find the given node by its name in the given XElement.
        /// Returns null, if nothing found.
        /// </summary>
        /// <param name="node">The node, containing the node.</param>
        /// <param name="name">The node name.</param>
        /// <returns>Returns null if node does not exist, else the node.</returns>
        public static XElement getNodeFromXElement(XElement node, in string name)
        {
            if(node != null)
            {
                foreach (XElement n in node.Elements())
                {
                    if (n.Name.LocalName.Equals(name))
                    {
                        return n;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to find the given node by its name, attribute and attribute value in the given XmlNode.
        /// Returns null, if nothing found.
        /// </summary>
        /// <param name="node">The node, containing the node.</param>
        /// <param name="name">The node name.</param>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="attributeValueRegex">The attribute value regular expression.</param>
        /// <returns>Returns null if node does not exist, else the node.</returns>
        public static XmlNode getChildNode(XmlNode node, in string name, in string attribute, Regex attributeValueRegex)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name.Equals(name))
                    {
                        if (attribute != null)
                        {
                            if (n.Attributes[attribute] != null && attributeValueRegex.IsMatch(n.Attributes[attribute].Value))
                            {
                                return n;
                            }
                        }
                        else
                        {
                            return n;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to find the given node by its name, attribute and attribute value in the given XmlNode.
        /// Returns null, if nothing found.
        /// </summary>
        /// <param name="node">The node, containing the node.</param>
        /// <param name="name">The node name.</param>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="attributeValue">The value of the attribute.</param>
        /// <returns>Returns null if node does not exist, else the node.</returns>
        public static XmlNode getChildNode(XmlNode node, in string name, in string attribute, in string attributeValue)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name.Equals(name))
                    {
                        if (attribute != null)
                        {
                            if (n.Attributes[attribute] != null && n.Attributes[attribute].Value.Equals(attributeValue))
                            {
                                return n;
                            }
                        }
                        else
                        {
                            return n;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to find the given node by its attribute and attribute value in the given XmlNode.
        /// Returns null, if nothing found.
        /// </summary>
        /// <param name="node">The node, containing the node.</param>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="attributeValue">The value of the attribute.</param>
        /// <returns>Returns null if node does not exist, else the node.</returns>
        public static XmlNode getChildNode(XmlNode node, in string attribute, in string attributeValue)
        {
            if (node != null && node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Attributes[attribute] != null && n.Attributes[attribute].Value.Equals(attributeValue))
                    {
                        return n;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// returns the xml attribute from the given node, if it exists. Else null.
        /// </summary>
        /// <param name="node">The node containing the attribute.</param>
        /// <param name="name">The attribute name.</param>
        /// <returns>Returns the attribute if found else null.</returns>
        public static XmlAttribute getAttribute(XmlNode node, in string name)
        {
            if (node.Attributes != null)
            {
                return node.Attributes[name];
            }
            return null;
        }

        /// <summary>
        /// Tries to pars the given string to a boolean.
        /// '  1   '/'1'/'   true   '/'true'/'   True   '/'True' result in true.
        /// Everything else results in false.
        /// </summary>
        /// <param name="s">The string containing the boolean.</param>
        /// <returns>The boolean representation of the given string.</returns>
        public static bool tryParseToBool(in string s)
        {
            if (s != null)
            {
                int i = -1;
                bool b = false;
                if (int.TryParse(s, out i))
                {
                    if (i == 1)
                    {
                        return true;
                    }
                }
                else if (bool.TryParse(s, out b))
                {
                    return b;
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
