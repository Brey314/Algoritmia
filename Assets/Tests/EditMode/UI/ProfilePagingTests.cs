using NUnit.Framework;

namespace Game.UI.Tests
{
    public class ProfilePagingTests
    {
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 2)]
        [TestCase(6, 2)]
        [TestCase(7, 3)]
        [TestCase(8, 3)]
        public void ProfilePaging_RF02_ContarPaginasDeTresPerfiles(int total, int expected) =>
            Assert.That(ProfilePaging.PageCount(total, 3), Is.EqualTo(expected));

        [Test]
        public void ProfilePaging_RF02_TrasBorrarLaUltimaFilaDeLaUltimaPaginaVuelveALaAnterior() =>
            Assert.That(ProfilePaging.ClampPage(1, 3, 3), Is.EqualTo(0));

        [Test]
        public void ProfilePaging_RF02_NuncaDevuelveUnaPaginaNegativaNiPosteriorALaUltima()
        {
            Assert.That(ProfilePaging.ClampPage(-1, 6, 3), Is.EqualTo(0));
            Assert.That(ProfilePaging.ClampPage(5, 6, 3), Is.EqualTo(1));
        }
    }
}
