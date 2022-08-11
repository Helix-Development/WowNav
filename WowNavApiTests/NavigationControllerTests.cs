using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using WowNavApi.Controllers;
using WowNavApi.Services;
using WowNavBase;

namespace WowNavApiTests
{
    [TestClass]
    public class NavigationControllerTests
    {
        private readonly Mock<ILogger<NavigationController>> mockLogger = new();
        private readonly Mock<INavigationService> mockNavigationService = new();

        [TestMethod]
        public void CalculatePath_StartNull_BadRequest()
        {
            var controller = BuildController();
            var parameters = GetCalculatePathParameters();
            parameters.Start = null;
            var result = controller.CalculatePath(parameters);
            var badRequestResult = result as BadRequestObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.AreEqual($"{nameof(parameters.Start)} cannot be null.", badRequestResult.Value as string);
        }

        [TestMethod]
        public void CalculatePath_EndNull_BadRequest()
        {
            var controller = BuildController();
            var parameters = GetCalculatePathParameters();
            parameters.End = null;
            var result = controller.CalculatePath(parameters);
            var badRequestResult = result as BadRequestObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.AreEqual($"{nameof(parameters.End)} cannot be null.", badRequestResult.Value as string);
        }

        [TestMethod]
        public void CalculatePath_NavigationServiceThrowsException_InternalServerError()
        {
            var parameters = GetCalculatePathParameters();

            mockNavigationService
                .Setup(m => m.CalculatePath(parameters.MapId, parameters.Start, parameters.End, parameters.StraightPath))
                .Throws(new InvalidOperationException("Test exception"));

            var controller = BuildController();
            var result = controller.CalculatePath(parameters);
            var objectResult = result as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }

        [TestMethod]
        public void CalculatePath_Success()
        {
            var parameters = GetCalculatePathParameters();

            var path = new Position[]
            {
                new Position(1.0f, 2.0f, 3.0f),
                new Position(4.0f, 5.0f, 6.0f),
                new Position(7.0f, 8.0f, 9.0f)
            };

            mockNavigationService
                .Setup(m => m.CalculatePath(parameters.MapId, parameters.Start, parameters.End, parameters.StraightPath))
                .Returns(path);

            var controller = BuildController();
            var result = controller.CalculatePath(parameters);
            var okObjectResult = result as ObjectResult;
            var resultPath = okObjectResult.Value as Position[];

            Assert.AreEqual((int)HttpStatusCode.OK, okObjectResult.StatusCode);
            Assert.AreEqual(3, resultPath.Length);
            Assert.IsTrue(resultPath[0].X == path[0].X && resultPath[0].Y == path[0].Y && resultPath[0].Z == path[0].Z);
            Assert.IsTrue(resultPath[1].X == path[1].X && resultPath[1].Y == path[1].Y && resultPath[1].Z == path[1].Z);
            Assert.IsTrue(resultPath[2].X == path[2].X && resultPath[2].Y == path[2].Y && resultPath[2].Z == path[2].Z);
        }

        private NavigationController BuildController()
        {
            return new NavigationController(mockLogger.Object, mockNavigationService.Object);
        }

        private static CalculatePathParameters GetCalculatePathParameters()
        {
            return new CalculatePathParameters
            {
                MapId = 1,
                Start = new Position(13.0f, 587.0f, 143.3f),
                End = new Position(23.0f, 541.3f, 133.3f),
                StraightPath = false
            };
        }
    }
}
