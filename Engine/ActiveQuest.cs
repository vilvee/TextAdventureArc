using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Represents a quest in the game.
    /// </summary>
    public class ActiveQuest
    {

        private Quest _details; // Holds the details of the quest.
        private bool _isCompleted; // Indicates whether the quest is completed or not.

        /// <summary>
        /// Gets or sets the details of the quest.
        /// </summary>
        public Quest Details
        {
            get => _details;
            set
            {
                _details = value;
                OnPropertyChanged("Details"); // Notify that the 'Details' property has changed.
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the quest is completed.
        /// </summary>
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged("IsCompleted"); // Notify that the 'IsCompleted' property has changed.
                OnPropertyChanged("Name"); // Notify that the 'Name' property has changed.
            }
        }

        /// <summary>
        /// Gets the name of the quest.
        /// </summary>
        public string Name => Details.Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveQuest"/> class with the specified details.
        /// </summary>
        /// <param name="details">The details of the quest.</param>
        public ActiveQuest(Quest details)
        {
            Details = details;
            IsCompleted = false;
        }

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event with the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}