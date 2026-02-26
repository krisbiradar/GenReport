using CoreDdd.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GenReport.DB.Domain.Entities.Business
{
    /// <summary>
    /// This class represents a module within the system.
    /// </summary>
    [Table("modules")] // Table name mapping
    public class Module : Entity<long>, IAggregateRoot
    {
        /// <summary>
        /// The name of the module.
        /// </summary>
        [Column("name")] // Column name mapping
        [Required]  // Enforces non-null value
        public required string Name { get; set; }

        /// <summary>
        /// A description of the module's functionality.
        /// </summary>
        [Column("description")] // Column name mapping
        [Required]  // Enforces non-null value
        public required string Description { get; set; }

        /// <summary>
        /// The CSS class used to display an icon for the module. (Optional, defaults to "Default Icon Class")
        /// </summary>
        [Column("icon_class")] // Column name mapping
        public string IconClass { get; set; } = "Default Icon Class";

        /// <summary>
        /// The date and time the module was created.
        /// </summary>
        [Column("created_at")] // Column name mapping
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The date and time the module was last updated.
        /// </summary>
        [Column("updated_at")] // Column name mapping
        public DateTime UpdatedAt { get; set; }
    }
}