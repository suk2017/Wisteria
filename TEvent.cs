using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch
{
    class TEvent
    {
        /*=======基本内容=======*/
        /// <summary>
        /// 主要内容
        /// </summary>
        public string content
        {
            get
            {
                return m_content;
            }
            set
            {
                content = value;
            }
        }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime time
        {
            get
            {
                return m_time;
            }
            set
            {
                m_time = value;
            }
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        /// <summary>
        /// 持续时间
        /// </summary>
        public TimeSpan duration
        {
            get
            {
                return DateTime.Now - m_time;
            }
        }

        /// <summary>
        /// 优先级 0是正常 1高优先 -1低优先
        /// </summary>
        public int priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                priority = value;
            }
        }

        /// <summary>
        /// 是否可见 用于删除    
        /// </summary>
        public bool visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }

        private string m_content;
        private DateTime m_time;
        private bool m_enabled;
        private int m_priority;
        private bool m_visible;

        /*=======方法=======*/
        public TEvent(string _content)
        {
            m_content = _content;
            m_time = DateTime.Now;
            m_enabled = true;
            m_priority = 0;
            m_visible = true;
        }


    }
    
}
