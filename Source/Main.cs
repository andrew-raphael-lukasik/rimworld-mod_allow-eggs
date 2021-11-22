using System.Collections.Generic;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AllowEggs
{
	public class AllowEggsMapComponent : MapComponent
	{

		const int k_ticks_threshold = 3317;
		int _ticks = 0;
		/// <remarks> A HashSet to list corpses that were allowed already so we don't do that again. </remarks>
		HashSet<Thing> _allowedAlready;

		public AllowEggsMapComponent ( Map map ) : base(map)
		{
			_allowedAlready = new HashSet<Thing>();
		}

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
			Faction playerFaction = Faction.OfPlayer;
			var list = map.listerThings.ThingsInGroup( ThingRequestGroup.FoodSource );
			for( int i=0 ; i<list.Count ; i++ )
			{
				Thing thing = list[i];
				if(
						thing.IsForbidden(playerFaction)
					&&	!_allowedAlready.Contains(thing)
					&&	thing.def.defName.StartsWith("Egg")
					&&	!thing.Fogged()
				)
				{
					_allowedAlready.Add(thing);
					thing.SetForbidden( false );
				}
			}
		}

		public override void ExposeData ()
		{
			Scribe_Collections.Look( ref _allowedAlready , false , nameof(_allowedAlready) , LookMode.Reference );
			base.ExposeData();
		}

	}
}
