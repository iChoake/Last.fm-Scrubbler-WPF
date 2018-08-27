﻿using IF.Lastfm.Core.Objects;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using IF.Lastfm.Core.Scrobblers;
using Scrubbler.Scrobbling.Scrobbler;
using Scrubbler.Scrobbling;
using Scrubbler.Helper;
using IF.Lastfm.Core.Api.Enums;

namespace Scrubbler.Test.ScrobblerTests
{
  /// <summary>
  /// Tests for the <see cref="ManualScrobbleViewModel"/>.
  /// </summary>
  [TestFixture]
  class ManualScrobblerTest
  {
    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.Scrobble"/>.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task ScrobbleTest()
    {
      Scrobble expected = new Scrobble("TestArtist", "TestAlbum", "TestTrack", DateTime.Now) { AlbumArtist = "TestAlbumArtist", Duration = TimeSpan.FromSeconds(30) };

      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>();
      Scrobble actual = null;
      scrobblerMock.Setup(i => i.ScrobbleAsync(It.IsAny<Scrobble>(), false)).Callback<Scrobble, bool>((s, c) => actual = s)
                                                                            .Returns(Task.Run(() => new ScrobbleResponse(LastResponseStatus.Successful)));

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        Artist = expected.Artist,
        Album = expected.Album,
        Track = expected.Track,
        AlbumArtist = expected.AlbumArtist,
        Duration = expected.Duration.Value
      };
      vm.ScrobbleTimeVM.UseCurrentTime = false;
      vm.ScrobbleTimeVM.Time = expected.TimePlayed.DateTime;

      await vm.Scrobble();

      Assert.That(actual.IsEqualScrobble(expected), Is.True);
    }

    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.CanScrobble"/>
    /// and <see cref="ManualScrobbleViewModel.CanPreview"/> is false
    /// when no auth is done.
    /// </summary>
    [Test]
    public void CanScrobbleAndPreviewNoAuthTest()
    {
      // given: ManualScrobbleViewModel without auth
      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>();
      scrobblerMock.Setup(s => s.IsAuthenticated).Returns(false);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        Artist = "TestArtist",
        Track = "TestTrack"
      };

      // then: CanScrobble should be false
      Assert.That(vm.CanScrobble, Is.False);
      // CanPreview should be true
      Assert.That(vm.CanPreview, Is.True);

      // make sure it really was the auth
      scrobblerMock.Setup(s => s.IsAuthenticated).Returns(true);
      Assert.That(vm.CanScrobble, Is.True);
    }

    /// <summary>
    /// Tests the <see cref="ManualScrobbleViewModel.CanScrobble"/> 
    /// and <see cref="ManualScrobbleViewModel.CanPreview"/> is false
    /// when no artist is given.
    /// </summary>
    [Test]
    public void CanScrobbleAndPreviewNoArtistTest()
    {
      // given: ManualScrobbleViewModel without given artist.
      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>();
      scrobblerMock.Setup(s => s.IsAuthenticated).Returns(true);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        Track = "TestTrack"
      };

      // then: CanScrobble and CanPreview should be false
      Assert.That(vm.CanScrobble, Is.False);
      Assert.That(vm.CanPreview, Is.False);

      // make sure it really was the artist
      vm.Artist = "TestArtist";
      Assert.That(vm.CanScrobble, Is.True);
      Assert.That(vm.CanPreview, Is.True);
    }

    /// <summary>
    /// Tests that <see cref="ManualScrobbleViewModel.CanScrobble"/> 
    /// and <see cref="ManualScrobbleViewModel.CanPreview"/> is false
    /// when no track is given.
    /// </summary>
    [Test]
    public void CanScrobbleAndPreviewNoTrackTest()
    {
      // given: ManualScrobbleViewModel without given artist.
      Mock<IUserScrobbler> scrobblerMock = new Mock<IUserScrobbler>();
      scrobblerMock.Setup(s => s.IsAuthenticated).Returns(true);

      Mock<IExtendedWindowManager> windowManagerMock = new Mock<IExtendedWindowManager>(MockBehavior.Strict);

      ManualScrobbleViewModel vm = new ManualScrobbleViewModel(windowManagerMock.Object)
      {
        Scrobbler = scrobblerMock.Object,
        Artist = "TestArtist"
      };

      // then: CanScrobble and CanPreview should be false
      Assert.That(vm.CanScrobble, Is.False);
      Assert.That(vm.CanPreview, Is.False);

      // make sure it really was the track
      vm.Track = "TestTrack";
      Assert.That(vm.CanScrobble, Is.True);
      Assert.That(vm.CanPreview, Is.True);
    }
  }
}