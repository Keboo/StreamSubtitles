using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StreamSubtitles.Tests
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void OnStop_ValuesCleared()
        {
            //Arrange
            var vm = new MainWindowViewModel(Factory.GetRecognizerFactory());
            vm.Recognizer = Factory.GetRecognizer();
            vm.PendingText = "Some Text";

            //Act
            vm.StopCommand.Execute(null);

            //Assert
            Assert.IsNull(vm.Recognizer);
            Assert.IsNull(vm.PendingText);
        }

        [TestMethod]
        public void OnStart_CreatesRecognizer()
        {
            //Arrange
            var vm = new MainWindowViewModel(Factory.GetRecognizerFactory());

            //Act
            vm.StartCommand.Execute(null);

            //Assert
            Assert.IsNotNull(vm.Recognizer);
        }

        [TestMethod]
        public void OnRecognizing_SetsPendingText()
        {
            //Arrange
            var vm = new MainWindowViewModel(Factory.GetRecognizerFactory("Foo"));

            IPropertyChanges<string> pendingTextChanges = 
                vm.WatchPropertyChanges<string>(nameof(MainWindowViewModel.PendingText));

            //Act
            vm.StartCommand.Execute(null);

            //Assert
            CollectionAssert.AreEqual(new[] {"Foo ", null}, pendingTextChanges.ToArray());
        }
    }
}
