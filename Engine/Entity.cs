using System.ComponentModel;

namespace Engine
{
    /// <summary>
    /// Represents an entity with hit points that can be damaged or killed.
    /// </summary>
    public class Entity : INotifyPropertyChanged
    {
        private int _currentHitPoints;

        /// <summary>
        /// Gets or sets the current hit points of the entity.
        /// </summary>
        public int CurrentHitPoints
        {
            get => _currentHitPoints;
            set
            {
                _currentHitPoints = value;
                OnPropertyChanged("CurrentHitPoints");
            }
        }

        /// <summary>
        /// Gets or sets the maximum hit points of the entity.
        /// </summary>
        public int MaximumHitPoints { get; set; }

        /// <summary>
        /// Determines if the entity is dead.
        /// </summary>
        public bool IsDead => CurrentHitPoints <= 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with the specified hit points.
        /// </summary>
        /// <param name="currentHitPoints">The current hit points of the entity.</param>
        /// <param name="maximumHitPoints">The maximum hit points of the entity.</param>
        public Entity(int currentHitPoints, int maximumHitPoints)
        {
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
