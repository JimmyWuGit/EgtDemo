using System.ComponentModel.DataAnnotations;

namespace EgtDemo.Model
{
    /// <summary>
    /// demo类
    /// </summary>
    public class Demo
    {
        /// <summary>
        /// Demo ID
        /// </summary>
        [Required(ErrorMessage ="{0}是必须的")]
        public int Id { get; set; }

        /// <summary>
        /// Demo名称
        /// </summary>
        [Required(ErrorMessage ="{0}是必须的")]
        [StringLength(3,ErrorMessage ="{0}至少为{1}个字符！")]
        public string Name { get; set; }
    }
}
