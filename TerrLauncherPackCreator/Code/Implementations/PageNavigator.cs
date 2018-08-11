using System;
using TerrLauncherPackCreator.Code.Interfaces;

namespace TerrLauncherPackCreator.Code.Implementations
{
    public class PageNavigator : IPageNavigator
    {
        private readonly Action _navigateBackward;
        private readonly Action _navigateForward;

        public PageNavigator(Action navigateBackward, Action navigateForward)
        {
            _navigateBackward = navigateBackward ?? throw new ArgumentNullException(nameof(navigateBackward));
            _navigateForward = navigateForward ?? throw new ArgumentNullException(nameof(navigateForward));
        }

        public void NavigateBackward()
        {
            _navigateBackward();
        }

        public void NavigateForward()
        {
            _navigateForward();
        }
    }
}
