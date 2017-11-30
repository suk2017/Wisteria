using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watch
{
    using System;


    class TEventList
    {
        /// <summary>
        /// 列表 每次调用时都会检查是否需要整理 谨慎调用
        /// </summary>
        public TEvent[] list
        {
            get
            {
                if (!compressed)
                {
                    _Compress();
                }
                return m_list;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }

        /// <summary>
        /// 建立时间
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
        /// 持续至今的时间
        /// </summary>
        public TimeSpan duration
        {
            get
            {
                return DateTime.Now - m_time;
            }
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public int priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;

            }
        }

        /// <summary>
        /// 返回当前的最大长度
        /// </summary>
        public int maxLength
        {
            get
            {
                return m_list.Length;
            }
        }

        /// <summary>
        /// 返回当前时序链中的元素个数
        /// </summary>
        public int length
        {
            get
            {
                return m_length;
            }
        }

        /// <summary>
        /// 激活的
        /// </summary>
        public bool enabled
        {
            get
            {
                return m_editable;
            }
            set
            {
                m_editable = value;
            }
        }

        /// <summary>
        /// 是否可见
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

        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool editable
        {
            get
            {
                return m_editable;
            }
            set
            {
                m_editable = value;
            }
        }

        /// <summary>
        /// 是否已压缩
        /// </summary>
        public bool compressed
        {
            get
            {
                return m_compressed;
            }
        }

        const int TEVENT_LIST_SIZE = 500;//默认长度
        const int TEVENT_LIST_STEP = 500;//再次增加时的长度
        private TEvent[] m_list;
        private string m_title;
        private DateTime m_time;
        private int m_priority;
        private int m_length;
        private bool m_enabled;
        private bool m_visible;
        private bool m_editable;
        private bool m_compressed;

        /*=======公有方法=======*/
        public TEventList(string _title, int _priority = 0)
        {
            m_list = new TEvent[TEVENT_LIST_SIZE];
            m_title = _title;
            m_time = DateTime.Now;
            m_priority = _priority;
            m_length = 0;
            m_enabled = true;
            m_visible = true;
            m_editable = true;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public void _Add(TEvent e)
        {
            ++m_length;
            if (length >= maxLength)//正常情况下是等于
            {
                _Expand();
            }
            m_list[length] = e;
            int temp = e.priority;
            e.priority = m_list[length-1].priority;//TODO 优化
            _Sort(length, temp);
        }

        /// <summary>
        /// 移除 只是隐藏起来 会在Compress中真正删除
        /// </summary>
        /// <param name="index"></param>
        public void _Remove(int index)
        {
            m_list[index].visible = false;
            m_compressed = false;
        }

        /// <summary>
        /// 移动顺序 两个参数的参考系都是源地址参考 序号越小越靠前
        /// </summary>
        /// <param name="src">源地址</param>
        /// <param name="des">目的地址 正数为向上</param>
        /// <param name="relative">是否为相对寻址 </param>
        public void _Move(int src, int des, bool relative = false)
        {
            TEvent temp = m_list[src];//临时变量 用于交换
            int tar = des;
            if (relative)//如果是相对寻址
            {
                tar = src - des;
            }
            if (tar == src)//如果目标与源点是同一位置
            {
                return;
            }
            if (tar > src)//如果目标在源点下方
            {
                for (int i = src; i < tar; ++i)
                {
                    m_list[i] = m_list[i + 1];
                }
                m_list[tar] = temp;
            }
            else//如果目标在原点上方
            {
                for (int i = src; i > tar; --i)
                {
                    m_list[i] = m_list[i - 1];
                }
                m_list[tar] = temp;
            }
        }

        /// <summary>
        /// 按照优先级进行排序 事实上是每次修改优先级都进行一次整理 而不是整个排序
        /// </summary>
        public void _Sort(int index, int _priority)
        {
            if (m_list[index].priority == _priority)
            {
                return;
            }
            int _length = length;
            int _p = m_list[index].priority;
            if (m_list[index].priority > _priority)//如果降低了优先级
            {
                for (int i = index; i < _length; ++i)
                {
                    if (_p > m_list[i].priority)//如果刚好停在>=自己优先级的队列后面
                    {
                        _Move(index, i);
                    }
                }
            }
            else//如果升高了优先级
            {
                for (int i = index; i > 0; --i)
                {
                    if (_p <= m_list[i].priority)
                    {
                        _Move(index, i + 1);
                    }
                }
            }
        }

        /*=======私有方法=======*/
        /// <summary>
        /// 压缩 1将所有隐藏的都真正删掉
        /// </summary>
        private void _Compress()
        {
            int _offset = 0;
            int _length = length;
            for (int i = 0; i < _length; ++i)
            {
                if (m_list[i].visible == false)
                {
                    ++_offset;
                }
                else
                {
                    m_list[i - _offset] = m_list[i];
                }
            }

            m_compressed = true;
        }

        /// <summary>
        /// 扩容
        /// </summary>
        private void _Expand()
        {
            int _length = m_list.Length;
            TEvent[] newList = new TEvent[_length + TEVENT_LIST_STEP];
            for (int i = 0; i < _length; ++i)
            {
                newList[i] = m_list[i];
            }
            m_list = newList;
        }
    }


}
