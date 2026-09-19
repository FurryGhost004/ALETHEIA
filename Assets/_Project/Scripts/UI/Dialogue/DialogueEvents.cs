public struct StartInterrogationEvent
{
    public SuspectNPC Suspect { get; private set; }

    public StartInterrogationEvent(SuspectNPC suspect)
    {
        Suspect = suspect;
    }
}
