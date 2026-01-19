using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBilansuKalorycznego.Model
{
    public class ActivityEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StartTime { get; set; } = DateTime.Now;

        // Powiązanie z definicją aktywności
        public PhysicalActivity Activity { get; set; } = new PhysicalActivity();

        // Czas trwania
        public TimeSpan Duration { get; set; }

        public double CalculateCaloriesBurned()
        {
            if (Activity == null) return 0;
            return Activity.CalculateCaloriesBurned(Duration);
        }
        public double CaloriesDisplay => CalculateCaloriesBurned();
    }
}
