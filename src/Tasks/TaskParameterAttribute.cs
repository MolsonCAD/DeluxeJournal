namespace DeluxeJournal.Tasks
{
    /// <summary>Attribute to mark a TaskFactory property as a TaskParameter.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskParameterAttribute : Attribute
    {
        /// <summary>Parameter name.</summary>
        public string Name { get; set; }

        /// <summary>Parser tag for populating special parameter values.</summary>
        public string Tag { get; set; }

        /// <summary>Is this task required?</summary>
        /// <remarks>If false, IsValid is always true.</remarks>
        public bool Required { get; set; }

        /// <summary>If false, this parameter is not exposed to the user in the task options menu.</summary>
        public bool Hidden { get; set; }

        /// <summary>Constraints on the parameter in the IsValid check.</summary>
        /// <remarks>
        /// IMPORTANT: This does NOT prevent the parameter value from being set if the value does not meet
        /// the Constraint requirements. Additional checks must be made by the programmer to enforce the
        /// constraints (if necessary).
        /// </remarks>
        public TaskParameter.Constraint Constraints { get; set; }

        public TaskParameterAttribute(string name)
        {
            Name = name;
            Tag = string.Empty;
            Required = true;
            Hidden = false;
            Constraints = TaskParameter.Constraint.NotEmpty;
        }
    }
}
