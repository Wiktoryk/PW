using ViewModel;

namespace PrezentacjaTests
{
    public class MainViewModelTest
    {
        [Test]
        public void ConstructorAndGettersTest()
        {
            ViewModelBase viewModel = new MainViewModel();

            Assert.IsNotNull(viewModel);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(((MainViewModel)viewModel).model);
                Assert.IsNotNull(((MainViewModel)viewModel).Balls);
                Assert.AreEqual(0, ((MainViewModel)viewModel).Balls.Count);
                Assert.AreEqual(0, ((MainViewModel)viewModel).ScenaWidth);
                Assert.AreEqual(0, ((MainViewModel)viewModel).ScenaHeight);
                Assert.AreEqual(0, ((MainViewModel)viewModel).BallsNumber);
                Assert.AreEqual(0, ((MainViewModel)viewModel).CurrentMaxBallsNumber);
                Assert.IsNotNull(((MainViewModel)viewModel).GenerateBallsCommand);
            });
        }

        [Test]
        public void SettersTest()
        {
            ViewModelBase viewModel = new MainViewModel();

            Assert.IsNotNull(viewModel);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0d, ((MainViewModel)viewModel).ScenaWidth, 0.01d);
                Assert.AreEqual(0d, ((MainViewModel)viewModel).ScenaHeight, 0.01d);
                Assert.AreEqual(0, ((MainViewModel)viewModel).BallsNumber);
                Assert.AreEqual(0, ((MainViewModel)viewModel).CurrentMaxBallsNumber);
            });

            ((MainViewModel)viewModel).ScenaWidth = 100d;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(100d, ((MainViewModel)viewModel).ScenaWidth, 0.01d);
                Assert.AreEqual(0d, ((MainViewModel)viewModel).ScenaHeight, 0.01d);
                Assert.AreEqual(0, ((MainViewModel)viewModel).CurrentMaxBallsNumber);
            });

            ((MainViewModel)viewModel).ScenaHeight = 200d;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(100d, ((MainViewModel)viewModel).ScenaWidth, 0.01d);
                Assert.AreEqual(200d, ((MainViewModel)viewModel).ScenaHeight, 0.01d);
                Assert.AreEqual(6d, ((MainViewModel)viewModel).CurrentMaxBallsNumber);
            });

            ((MainViewModel)viewModel).ScenaWidth = 2100d;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(2100d, ((MainViewModel)viewModel).ScenaWidth, 0.01d);
                Assert.AreEqual(200d, ((MainViewModel)viewModel).ScenaHeight, 0.01d);
                Assert.AreEqual(20, ((MainViewModel)viewModel).CurrentMaxBallsNumber);
            });

            ((MainViewModel)viewModel).BallsNumber = 1;
            Assert.AreEqual(1, ((MainViewModel)viewModel).BallsNumber);
        }
    }
}
