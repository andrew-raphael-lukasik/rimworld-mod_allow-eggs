using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AllowEggs
{
	public class AllowEggsMapComponent : MapComponent
	{

		const int k_ticks_threshold = 3317;
		int _ticks = 0;

		public AllowEggsMapComponent ( Map map ) : base(map) {}

		public override void MapComponentTick ()
		{
			if( ++_ticks==k_ticks_threshold )
			{
				Task.Run( Routine );
				_ticks = 0;
			}
		}

		/// <remarks> List is NOT thread-safe so EXPECT it can be changed by diffent CPU thread, mid-execution, anytime here.</remarks>
		void Routine ()
		{
			var playerFaction = Faction.OfPlayer;
			var list = map.listerThings.ThingsInGroup( ThingRequestGroup.FoodSource );
			for( int i=0 ; i<list.Count ; i++ )
			{
				Thing thing = list[i];
				if( thing.def.defName.StartsWith("Egg") && thing.IsForbidden(playerFaction) )
					thing.SetForbidden( false );
			}
		}

	}
}
