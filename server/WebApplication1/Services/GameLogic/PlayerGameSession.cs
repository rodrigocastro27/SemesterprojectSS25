    using WebApplication1.Models;
    using WebApplication1.Models.Abilities;
    using WebApplication1.Services.Messaging;

    namespace WebApplication1.Services;

    public class PlayerGameSession(Player player, GameSession gameSession)
    {
        private Player _player = player;
        private GameSession _gameSession = gameSession;

        private bool _isVisible = true;
        
        public readonly Dictionary<string, System.Text.Json.JsonElement> TaskUpdates = new();

        public bool IsEliminated { get; private set; }


        private List<IAbility> _abilities = new();

        public void GrantAbility(IAbility ability)
        {
            _abilities.Add(ability);
        }

        public bool HasAbility(IAbility ability)
        {
            return _abilities.Contains(ability);
        }
        
        public IAbility GetAbilityFromString(string abilityName)
        {
            foreach (var ability in _abilities)
            {
                Console.WriteLine("comparing " + abilityName + " to " + ability.Type.ToString());
                if (ability.Type.ToString() == abilityName)
                {
                    return ability;
                }
            }

            return null;
        }

        public async Task UseAbility(IAbility ability)
        {
            if (!HasAbility(ability)) return;

            try
            {
                await ability.ApplyEffectAsync(_gameSession, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while using ability {ability.GetType().Name}: {ex.Message}");
            }
            finally
            {
                _abilities.Remove(ability); // Clean removal after use
                ability.Dispose();
            }
        }

        public List<IAbility> GetAbilityList() => _abilities;
        
        public void MarkUpdateReceived(string taskName, System.Text.Json.JsonElement info)
        {
            TaskUpdates[taskName] = info;
        }

        public bool HasSentUpdate(string taskName)
        {
            return TaskUpdates.TryGetValue(taskName, out var value) && value.ValueKind != System.Text.Json.JsonValueKind.Undefined;;
        }
        public void Eliminate()
        {
            IsEliminated = true;
            Console.WriteLine($"{player.Name} has been eliminated from the game.");
            _ = GameMessageSender.SendEliminatedPlayer(player);
        }
        public System.Text.Json.JsonElement GetInfoFrom(string taskName)
        {
            TaskUpdates.TryGetValue(taskName, out var value);
            return value!;
        }

        public Player? GetPlayer() => player;

        public void SetVisible(bool visible) => _isVisible = visible;
        
        public bool CheckVisibility()
        {
            if (IsEliminated) return false;
            
            if (_isVisible == false)
            {
                return false;
            }
            return true;
        }

    }