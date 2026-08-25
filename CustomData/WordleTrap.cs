// TODO: Timer?

namespace Freedom_Planet_2_Archipelago.CustomData
{
    internal class WordleTrap : MonoBehaviour
    {
        /// <summary>
        /// The player's answer.
        /// </summary>
        public static string playerAnswer = "     ";

        /// <summary>
        /// Array of possible answers, sourced from: https://www.rockpapershotgun.com/wordle-past-answers
        /// </summary>
        public static string[] answers = ["ABACK", "ABASE", "ABATE", "ABBEY", "ABBOT", "ABHOR", "ABIDE", "ABOUT", "ABOVE", "ABYSS", "ACORN", "ACRID", "ACTOR", "ACUTE", "ADAGE", "ADAPT", "ADEPT", "ADMIN", "ADMIT", "ADOBE", "ADOPT", "ADORE", "ADULT", "AFFIX", "AFOOT", "AFTER", "AGAIN", "AGAPE", "AGATE", "AGENT", "AGILE", "AGING", "AGLOW", "AGONY", "AGREE", "AHEAD", "AISLE", "ALARM", "ALBUM", "ALERT", "ALIBI", "ALIEN", "ALIGN", "ALIKE", "ALIVE", "ALLOT", "ALLOW", "ALLOY", "ALOFT", "ALOHA", "ALONE", "ALONG", "ALOOF", "ALOUD", "ALLEY", "ALPHA", "ALTAR", "ALTER", "AMASS", "AMAZE", "AMBER", "AMBLE", "AMEND", "AMISS", "AMONG", "AMPLE", "AMPLY", "AMUSE", "ANGEL", "ANGER", "ANGLE", "ANGRY", "ANGST", "ANKLE", "ANNEX", "ANNOY", "ANODE", "ANTIC", "ANVIL", "AORTA", "APART", "APHID", "APPLE", "APPLY", "APRON", "APTLY", "ARBOR", "ARDOR", "ARGUE", "ARISE", "AROMA", "ARROW", "ARTSY", "ASCOT", "ASHEN", "ASIDE", "ASKEW", "ASPIC", "ASSAY", "ASSET", "ATLAS", "ATOLL", "ATONE", "ATRIA", "ATTIC", "AUDIO", "AUDIT", "AVAIL", "AVERT", "AVIAN", "AVOID", "AWAIT", "AWARD", "AWAKE", "AWARE", "AWASH", "AWFUL", "AWOKE", "AXIOM", "AZURE", "BACON", "BADGE", "BADLY", "BAGEL", "BAKER", "BALER", "BALMY", "BALSA", "BANAL", "BANJO", "BARGE", "BARON", "BASIC", "BASIL", "BASIN", "BASIS", "BASTE", "BATON", "BATCH", "BATHE", "BATTY", "BAWDY", "BAYOU", "BEACH", "BEADY", "BEARD", "BEAST", "BEAUT", "BEEFY", "BEFIT", "BEGET", "BEGIN", "BEGUN", "BEING", "BELCH", "BELIE", "BELLE", "BELLY", "BELOW", "BENCH", "BERET", "BERTH", "BESET", "BEVEL", "BICEP", "BILGE", "BINGE", "BIOME", "BIRCH", "BIRTH", "BLACK", "BLADE", "BLAME", "BLAND", "BLANK", "BLARE", "BLAST", "BLAZE", "BLEAK", "BLEAT", "BLEED", "BLEEP", "BLEND", "BLIMP", "BLINK", "BLISS", "BLOCK", "BLOKE", "BLOND", "BLOOM", "BLOWN", "BLUFF", "BLUNT", "BLURB", "BLURT", "BLUSH", "BOARD", "BOAST", "BONGO", "BONUS", "BOOBY", "BOOST", "BOOTH", "BOOTY", "BOOZE", "BOOZY", "BORAX", "BORNE", "BOSSY", "BOUGH", "BOXER", "BRACE", "BRAID", "BRAIN", "BRAKE", "BRAND", "BRASH", "BRASS", "BRAVE", "BRAVO", "BRAWN", "BREAD", "BREAK", "BREED", "BRIAR", "BRIBE", "BRIDE", "BRIEF", "BRINE", "BRING", "BRINK", "BRINY", "BRISK", "BROAD", "BROIL", "BROKE", "BROOD", "BROOK", "BROOM", "BROTH", "BROWN", "BRUSH", "BRUTE", "BUDDY", "BUDGE", "BUGGY", "BUGLE", "BUILD", "BUILT", "BULGE", "BULKY", "BULLY", "BUNCH", "BUNNY", "BURLY", "BURNT", "BUTTE", "BUYER", "BYLAW", "CABLE", "CACAO", "CACHE", "CACTI", "CADET", "CAMEL", "CAMEO", "CANAL", "CANDY", "CANNY", "CANOE", "CANON", "CAPER", "CARAT", "CARGO", "CAROL", "CAROM", "CARRY", "CARVE", "CATCH", "CATER", "CATTY", "CAULK", "CAUSE", "CEASE", "CEDAR", "CELLO", "CHAFE", "CHAIN", "CHAIR", "CHALK", "CHAMP", "CHANT", "CHAOS", "CHARD", "CHARM", "CHART", "CHASE", "CHASM", "CHEAP", "CHEAT", "CHECK", "CHEEK", "CHEER", "CHEST", "CHIDE", "CHIEF", "CHILD", "CHILI", "CHILL", "CHIME", "CHIRP", "CHOCK", "CHOIR", "CHOKE", "CHORD", "CHORE", "CHOSE", "CHUCK", "CHUMP", "CHUNK", "CHURN", "CHUTE", "CIDER", "CIGAR", "CINCH", "CIRCA", "CIVIC", "CIVIL", "CLACK", "CLAMP", "CLANG", "CLASH", "CLASP", "CLASS", "CLEAN", "CLEAR", "CLEFT", "CLERK", "CLICK", "CLIFF", "CLIMB", "CLING", "CLINK", "CLOAK", "CLOCK", "CLONE", "CLOSE", "CLOTH", "CLOUD", "CLOVE", "CLOWN", "CLUCK", "CLUMP", "CLUNG", "CLUNK", "COACH", "COAST", "COCOA", "COLIC", "COLON", "COMET", "COMFY", "COMMA", "CONCH", "CONDO", "CONIC", "CORAL", "CORER", "CORNY", "COUCH", "COULD", "COUNT", "COURT", "COVEN", "COVER", "COVET", "COWER", "COYLY", "CRAFT", "CRAMP", "CRANE", "CRANK", "CRASH", "CRASS", "CRATE", "CRAVE", "CRAWL", "CRAZE", "CRAZY", "CREAK", "CREAM", "CREDO", "CREED", "CREPE", "CREPT", "CREST", "CRIME", "CRIMP", "CRISP", "CROAK", "CROCK", "CRONE", "CROOK", "CROSS", "CROWD", "CROWN", "CRUDE", "CRUEL", "CRUMB", "CRUSH", "CRUST", "CRYPT", "CUBIC", "CUBIT", "CUMIN", "CURIO", "CURLY", "CURRY", "CURSE", "CURVE", "CYBER", "CYCLE", "CYNIC", "DADDY", "DAISY", "DALLY", "DANCE", "DANDY", "DATUM", "DAUNT", "DEATH", "DEBIT", "DEBUG", "DEBUT", "DECAL", "DECAY", "DECOR", "DECOY", "DECRY", "DEFER", "DEITY", "DELAY", "DELTA", "DELVE", "DEMON", "DEMUR", "DENIM", "DENSE", "DEPOT", "DEPTH", "DETER", "DETOX", "DEUCE", "DEVIL", "DIARY", "DICEY", "DIGIT", "DINER", "DINGO", "DINGY", "DIRGE", "DISCO", "DITTO", "DITTY", "DIVER", "DIVOT", "DIZZY", "DODGE", "DODGY", "DOGMA", "DOING", "DOLLY", "DONOR", "DONUT", "DOPEY", "DOUBT", "DOUGH", "DOWDY", "DOWEL", "DOWRY", "DOZEN", "DRAFT", "DRAIN", "DRAKE", "DRAMA", "DRAPE", "DRAWN", "DREAD", "DREAM", "DRIFT", "DRILL", "DRINK", "DRIVE", "DROLL", "DRONE", "DROOL", "DROOP", "DROVE", "DRUNK", "DRYER", "DUCHY", "DUMMY", "DUSKY", "DUSTY", "DUTCH", "DUVET", "DWARF", "DWELL", "DWELT", "EAGER", "EAGLE", "EARLY", "EARTH", "EASEL", "EATEN", "EBONY", "EDICT", "EDIFY", "EERIE", "EGRET", "EIGHT", "EJECT", "ELATE", "ELBOW", "ELDER", "ELFIN", "ELITE", "ELOPE", "ELUDE", "EMAIL", "EMBED", "EMBER", "EMCEE", "EMOJI", "EMPTY", "ENACT", "ENDOW", "ENEMA", "ENJOY", "ENNUI", "ENSUE", "ENTER", "ENTRY", "ENVOY", "EPOCH", "EPOXY", "EQUAL", "EQUIP", "ERASE", "ERODE", "ERROR", "ERUPT", "ESSAY", "ETHER", "ETHIC", "ETHOS", "ETUDE", "EVADE", "EVENT", "EVERY", "EVOKE", "EXACT", "EXALT", "EXCEL", "EXERT", "EXILE", "EXIST", "EXPEL", "EXTOL", "EXTRA", "EXULT", "FABLE", "FACET", "FAINT", "FAITH", "FALSE", "FANCY", "FARCE", "FAULT", "FAVOR", "FEAST", "FEIGN", "FENCE", "FERAL", "FERRY", "FETCH", "FETID", "FEVER", "FEWER", "FIBER", "FIELD", "FIEND", "FIERY", "FIFTH", "FIFTY", "FILET", "FILLY", "FINAL", "FINCH", "FINER", "FIRST", "FISHY", "FIXER", "FIZZY", "FJORD", "FLAIL", "FLAIR", "FLAKE", "FLAKY", "FLAME", "FLANK", "FLARE", "FLASH", "FLASK", "FLESH", "FLICK", "FLING", "FLINT", "FLIRT", "FLOAT", "FLOCK", "FLOOD", "FLOOR", "FLORA", "FLOSS", "FLOUR", "FLOUT", "FLOWN", "FLUFF", "FLUKE", "FLUME", "FLUNG", "FLUNK", "FLUTE", "FLYER", "FOAMY", "FOCAL", "FOCUS", "FOGGY", "FOIST", "FOLIO", "FOLLY", "FORAY", "FORCE", "FORGE", "FORGO", "FORTE", "FORTH", "FORTY", "FORUM", "FOUND", "FOYER", "FRAIL", "FRAME", "FRANK", "FREAK", "FRESH", "FRIED", "FRILL", "FRITZ", "FROCK", "FROND", "FRONT", "FROST", "FROTH", "FROWN", "FROZE", "FRUIT", "FUGUE", "FULLY", "FUNGI", "FUNKY", "FUNNY", "FUZZY", "GAMER", "GAMMA", "GAMUT", "GAUDY", "GAUGE", "GAUNT", "GAUZE", "GAVEL", "GAWKY", "GECKO", "GEESE", "GENIE", "GENRE", "GEODE", "GHOST", "GHOUL", "GIANT", "GIDDY", "GIRTH", "GIVEN", "GIZMO", "GLADE", "GLAND", "GLARE", "GLASS", "GLAZE", "GLEAM", "GLEAN", "GLIDE", "GLINT", "GLOAT", "GLOBE", "GLOOM", "GLORY", "GLOSS", "GLOVE", "GLYPH", "GNASH", "GNOME", "GOFER", "GOING", "GOLEM", "GONER", "GOODY", "GOOEY", "GOOFY", "GOOSE", "GORGE", "GOUGE", "GRACE", "GRADE", "GRAFT", "GRAIL", "GRAIN", "GRAND", "GRANT", "GRAPE", "GRAPH", "GRASP", "GRASS", "GRATE", "GRAVE", "GRAVY", "GREAT", "GREED", "GREEN", "GREET", "GRIEF", "GRIFT", "GRILL", "GRIME", "GRIMY", "GRIND", "GRIPE", "GROAN", "GROIN", "GROOM", "GROSS", "GROUP", "GROUT", "GROVE", "GROWL", "GROWN", "GRUEL", "GRUFF", "GUANO", "GUARD", "GUAVA", "GUESS", "GUEST", "GUIDE", "GUILD", "GUILE", "GUISE", "GULLY", "GUMBO", "GUMMY", "GUNKY", "GUPPY", "GUSTY", "HABIT", "HAIRY", "HALVE", "HANDY", "HAPPY", "HARDY", "HARSH", "HASTE", "HASTY", "HATCH", "HATER", "HAUNT", "HAVEN", "HAVOC", "HAZEL", "HEADY", "HEARD", "HEART", "HEATH", "HEAVE", "HEAVY", "HEFTY", "HEIST", "HELIX", "HELLO", "HENCE", "HERON", "HILLY", "HINGE", "HIPPO", "HITCH", "HOARD", "HOBBY", "HOIST", "HOLLY", "HOMER", "HONEY", "HORDE", "HORSE", "HOTEL", "HOUND", "HOUSE", "HOVEL", "HOVER", "HOWDY", "HUMAN", "HUMID", "HUMOR", "HUMPH", "HUNCH", "HUNKY", "HURRY", "HUTCH", "HYDRA", "HYENA", "HYPER", "ICING", "IDEAL", "IDIOM", "IDLER", "IGLOO", "IMAGE", "IMBUE", "IMPEL", "INANE", "INBOX", "INCUR", "INDEX", "INDIE", "INEPT", "INERT", "INFER", "INLAY", "INLET", "INNER", "INPUT", "INTER", "INTRO", "IONIC", "IRATE", "IRONY", "ISLET", "ISSUE", "ITCHY", "IVORY", "JAUNT", "JAZZY", "JELLY", "JERKY", "JEWEL", "JIFFY", "JOINT", "JOKER", "JOLLY", "JOUST", "JUDGE", "JUICE", "JUMBO", "JUMPY", "KARMA", "KAYAK", "KAZOO", "KEBAB", "KEFIR", "KHAKI", "KIOSK", "KNACK", "KNAVE", "KNEAD", "KNEEL", "KNELL", "KNELT", "KNIFE", "KNOCK", "KNOLL", "KNOWN", "KOALA", "KRILL", "LABEL", "LABOR", "LADEN", "LADLE", "LAGER", "LANCE", "LANKY", "LAPEL", "LAPSE", "LARGE", "LARVA", "LASER", "LASSO", "LATCH", "LATER", "LATHE", "LATTE", "LAUGH", "LAYER", "LEACH", "LEAFY", "LEAKY", "LEAPT", "LEARN", "LEASE", "LEASH", "LEAST", "LEAVE", "LEDGE", "LEECH", "LEERY", "LEFTY", "LEGAL", "LEGGY", "LEMON", "LEMUR", "LEVEL", "LEVER", "LIBEL", "LIGHT", "LIKEN", "LILAC", "LIMBO", "LIMIT", "LINEN", "LINER", "LINGO", "LITHE", "LIVER", "LIVID", "LLAMA", "LOATH", "LOBBY", "LOCAL", "LOCUS", "LODGE", "LOFTY", "LOGIC", "LOOPY", "LOOSE", "LORIS", "LORRY", "LOSER", "LOUSE", "LOUSY", "LOVER", "LOWER", "LOWLY", "LOYAL", "LUCID", "LUCKY", "LUMPY", "LUNAR", "LUNCH", "LUNGE", "LURID", "LUSTY", "LYING", "MACAW", "MACHO", "MADAM", "MADLY", "MAFIA", "MAGIC", "MAGMA", "MAIZE", "MAJOR", "MAKER", "MAMBO", "MANGA", "MANGO", "MANIA", "MANIC", "MANLY", "MANOR", "MAPLE", "MARCH", "MARRY", "MARSH", "MASON", "MASSE", "MATCH", "MATEY", "MATTE", "MAUVE", "MAVEN", "MAXIM", "MAYBE", "MAYOR", "MEALY", "MEANT", "MEDAL", "MEDIA", "MEDIC", "MELON", "MERCY", "MERGE", "MERIT", "MERRY", "METAL", "METER", "METRO", "MICRO", "MIDGE", "MIDST", "MIGHT", "MIMIC", "MINCE", "MINER", "MINTY", "MINUS", "MIRTH", "MISER", "MODAL", "MODEL", "MODEM", "MOGUL", "MOIST", "MOLAR", "MOLDY", "MOMMY", "MONEY", "MONTH", "MOOCH", "MOOSE", "MORAL", "MORPH", "MOSSY", "MOTEL", "MOTIF", "MOTOR", "MOTTO", "MOULT", "MOUNT", "MOURN", "MOUSE", "MOUTH", "MOVER", "MOVIE", "MUCKY", "MUGGY", "MULCH", "MUMMY", "MUNCH", "MURAL", "MURKY", "MUSHY", "MUSIC", "MUSTY", "MYRRH", "NADIR", "NAIVE", "NANNY", "NASAL", "NASTY", "NATAL", "NAVAL", "NAVEL", "NEEDY", "NEIGH", "NERDY", "NERVE", "NERVY", "NEVER", "NEWLY", "NICER", "NICHE", "NIECE", "NIGHT", "NINJA", "NINTH", "NOBLE", "NOBLY", "NOISE", "NOISY", "NOMAD", "NORTH", "NOTCH", "NOVEL", "NUDGE", "NURSE", "NYLON", "NYMPH", "OASIS", "OCCUR", "OCEAN", "OCTET", "ODDLY", "OFFAL", "OFFER", "OFTEN", "OLDER", "OLIVE", "OMEGA", "ONION", "ONSET", "OOMPH", "OPERA", "OPINE", "ORBIT", "ORDER", "ORGAN", "OTHER", "OTTER", "OUGHT", "OUNCE", "OUTDO", "OUTER", "OVATE", "OVERT", "OWNER", "OXIDE", "OZONE", "PAINT", "PANEL", "PANIC", "PAPAL", "PAPER", "PARER", "PARKA", "PARRY", "PARTY", "PASTA", "PATCH", "PATIO", "PATSY", "PATTY", "PAUSE", "PEACE", "PEACH", "PEARL", "PECAN", "PEDAL", "PENAL", "PENNE", "PERCH", "PERIL", "PERKY", "PESKY", "PETAL", "PETTY", "PHASE", "PHONE", "PHONY", "PHOTO", "PIANO", "PICKY", "PIECE", "PIETY", "PILOT", "PINCH", "PINEY", "PINKY", "PINTO", "PIOUS", "PIPER", "PIQUE", "PITCH", "PITHY", "PIXEL", "PIXIE", "PIZZA", "PLACE", "PLAID", "PLAIN", "PLAIT", "PLANE", "PLANK", "PLANT", "PLATE", "PLAZA", "PLEAD", "PLEAT", "PLUCK", "PLUMB", "PLUME", "PLUMP", "PLUNK", "POINT", "POISE", "POKER", "POLAR", "POLKA", "POLYP", "POPPY", "PORCH", "POSER", "POSIT", "POSSE", "POUND", "POUTY", "POWER", "PRANK", "PREEN", "PRESS", "PRICE", "PRICK", "PRIDE", "PRIME", "PRIMO", "PRIMP", "PRINT", "PRIOR", "PRISM", "PRIVY", "PRIZE", "PROBE", "PRONE", "PRONG", "PROOF", "PROSE", "PROUD", "PROVE", "PROWL", "PROXY", "PRUDE", "PRUNE", "PSALM", "PSHAW", "PUFFY", "PULPY", "PUPIL", "PUPPY", "PURGE", "PURSE", "PUTTY", "QUACK", "QUAIL", "QUAKE", "QUALM", "QUARK", "QUART", "QUASH", "QUEEN", "QUEER", "QUELL", "QUERY", "QUEST", "QUEUE", "QUICK", "QUIET", "QUILL", "QUILT", "QUIRK", "QUITE", "QUOTA", "QUOTE", "RABID", "RACER", "RADIO", "RAINY", "RAISE", "RAMEN", "RANCH", "RANGE", "RAPID", "RATIO", "RATTY", "RAYON", "REACH", "REACT", "READY", "REALM", "REBEL", "REBUS", "REBUT", "RECAP", "RECUR", "REFER", "REGAL", "REHAB", "RELAX", "RELAY", "RELIC", "REMIT", "RENEW", "REPAY", "REPEL", "REPLY", "RERUN", "RESIN", "RETCH", "RETRO", "RETRY", "REUSE", "REVEL", "REVUE", "RHINO", "RHYME", "RIDER", "RIDGE", "RIGHT", "RIGID", "RIPER", "RISEN", "RISER", "RIVAL", "RIVET", "ROACH", "ROBIN", "ROBOT", "ROCKY", "RODEO", "ROGUE", "ROOMY", "ROOST", "ROUGE", "ROUGH", "ROUND", "ROUSE", "ROUTE", "ROVER", "ROWDY", "ROWER", "ROYAL", "RUDDY", "RUDER", "RUGBY", "RUMBA", "RUPEE", "RURAL", "RUSTY", "SAINT", "SALAD", "SALLY", "SALSA", "SALTY", "SANDY", "SASSY", "SATIN", "SAUCY", "SAUNA", "SAUTE", "SAVOR", "SAVVY", "SCALD", "SCALE", "SCANT", "SCARE", "SCARF", "SCENE", "SCENT", "SCOFF", "SCOLD", "SCONE", "SCOOP", "SCOPE", "SCORE", "SCORN", "SCOUR", "SCOUT", "SCOWL", "SCRAM", "SCRAP", "SCRUB", "SCRUM", "SEDAN", "SEEDY", "SEGUE", "SENSE", "SEPIA", "SERIF", "SERUM", "SERVE", "SEVEN", "SEVER", "SHADE", "SHAFT", "SHAKE", "SHAKY", "SHALL", "SHAME", "SHANK", "SHAPE", "SHARD", "SHARE", "SHARP", "SHAVE", "SHAWL", "SHEAR", "SHEEP", "SHEET", "SHELF", "SHELL", "SHIFT", "SHILL", "SHINE", "SHIRE", "SHIRK", "SHOAL", "SHORE", "SHORN", "SHORT", "SHOUT", "SHOVE", "SHOWN", "SHOWY", "SHRED", "SHRUB", "SHRUG", "SHUCK", "SHUNT", "SHUSH", "SHYLY", "SIEGE", "SIGHT", "SILLY", "SINCE", "SINGE", "SIREN", "SISSY", "SITAR", "SIXTH", "SKATE", "SKIER", "SKIFF", "SKILL", "SKIMP", "SKIRT", "SKULL", "SKUNK", "SLANG", "SLATE", "SLEEK", "SLEEP", "SLICE", "SLICK", "SLIME", "SLING", "SLOPE", "SLOSH", "SLOTH", "SLUMP", "SLUNG", "SLUSH", "SMALL", "SMART", "SMASH", "SMEAR", "SMELL", "SMELT", "SMILE", "SMIRK", "SMITE", "SMITH", "SMOCK", "SMOKE", "SNACK", "SNAFU", "SNAIL", "SNAKE", "SNAKY", "SNARE", "SNARL", "SNEAK", "SNIDE", "SNIPE", "SNOOP", "SNORE", "SNORT", "SNOUT", "SOBER", "SOGGY", "SOLAR", "SOLID", "SOLVE", "SONAR", "SONIC", "SORRY", "SOUND", "SOUTH", "SOWER", "SPACE", "SPADE", "SPARE", "SPARK", "SPASM", "SPATE", "SPEAK", "SPEAR", "SPECK", "SPEED", "SPELL", "SPELT", "SPEND", "SPENT", "SPICE", "SPICY", "SPIEL", "SPIKE", "SPILL", "SPINE", "SPINY", "SPIRE", "SPITE", "SPLAT", "SPLIT", "SPOIL", "SPOKE", "SPOOF", "SPOOL", "SPOON", "SPORE", "SPORT", "SPOUT", "SPRAY", "SPRIG", "SPURT", "SQUAD", "SQUAT", "SQUID", "STACK", "STAFF", "STAGE", "STAID", "STAIN", "STAIR", "STAKE", "STALE", "STALL", "STAMP", "STAND", "STANK", "STARE", "STARK", "START", "STASH", "STATE", "STEAD", "STEAK", "STEAM", "STEED", "STEEL", "STEEP", "STEIN", "STERN", "STICK", "STIFF", "STILL", "STILT", "STING", "STINK", "STINT", "STOCK", "STOIC", "STOLE", "STOMP", "STONE", "STONY", "STOOD", "STOOL", "STORE", "STORK", "STORM", "STORY", "STOUT", "STOVE", "STRAP", "STRAW", "STRAY", "STRIP", "STRUT", "STUDY", "STUFF", "STUMP", "STUNG", "STUNT", "STYLE", "SUAVE", "SUEDE", "SUGAR", "SUITE", "SULKY", "SULLY", "SUMAC", "SUNNY", "SUPER", "SURER", "SURGE", "SURLY", "SUSHI", "SWAMI", "SWAMP", "SWATH", "SWEAT", "SWEEP", "SWEET", "SWELL", "SWILL", "SWINE", "SWING", "SWIRL", "SWISH", "SWOON", "SWOOP", "SWORD", "SWORN", "SWUNG", "SYRUP", "TABBY", "TABLE", "TABOO", "TACIT", "TACKY", "TAFFY", "TAKEN", "TALLY", "TALON", "TANGY", "TAPER", "TAPIR", "TARDY", "TASTE", "TASTY", "TAUNT", "TAUPE", "TAWNY", "TEACH", "TEARY", "TEASE", "TEDDY", "TEETH", "TEMPO", "TENOR", "TENTH", "TEPID", "TERSE", "TESTY", "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THICK", "THIEF", "THIGH", "THING", "THINK", "THIRD", "THORN", "THOSE", "THREE", "THREW", "THROB", "THROW", "THRUM", "THUMB", "THUMP", "THYME", "TIARA", "TIBIA", "TIDAL", "TIGER", "TILDE", "TIMER", "TINGE", "TIPSY", "TITAN", "TITHE", "TITLE", "TIZZY", "TOADY", "TOAST", "TODAY", "TODDY", "TOKEN", "TONIC", "TOOTH", "TOPAZ", "TOPIC", "TORCH", "TORSO", "TOTAL", "TOTEM", "TOUCH", "TOUGH", "TOWEL", "TOWER", "TOXIC", "TOXIN", "TRACE", "TRACK", "TRACT", "TRADE", "TRAIL", "TRAIN", "TRAIT", "TRASH", "TRAWL", "TREAT", "TREND", "TRIAD", "TRIAL", "TRIBE", "TRICE", "TRICK", "TRIPE", "TRITE", "TROLL", "TROOP", "TROPE", "TROUT", "TROVE", "TRUCK", "TRULY", "TRUSS", "TRUST", "TRUTH", "TRYST", "TUBER", "TULIP", "TUNIC", "TURBO", "TUTOR", "TWANG", "TWEAK", "TWEED", "TWEET", "TWICE", "TWINE", "TWIRL", "TWIST", "UDDER", "ULCER", "ULTRA", "UMBRA", "UNCLE", "UNDER", "UNDID", "UNDUE", "UNFED", "UNFIT", "UNIFY", "UNION", "UNITE", "UNITY", "UNLIT", "UNMET", "UNTIE", "UNTIL", "UNZIP", "UPPER", "UPSET", "URBAN", "USAGE", "USHER", "USING", "USUAL", "USURP", "UTTER", "UVULA", "VAGUE", "VALET", "VALID", "VALUE", "VALVE", "VAPID", "VAULT", "VEGAN", "VENOM", "VENUE", "VERGE", "VERSE", "VERVE", "VIDEO", "VIGOR", "VILLA", "VINYL", "VIOLA", "VIRAL", "VISIT", "VISOR", "VITAL", "VIVID", "VIXEN", "VOCAL", "VODKA", "VOGUE", "VOICE", "VOILA", "VOTER", "VOUCH", "VOWEL", "VYING", "WACKY", "WAFER", "WAGON", "WAIST", "WALTZ", "WASTE", "WATCH", "WATER", "WAXEN", "WAVER", "WEARY", "WEAVE", "WEDGE", "WEEDY", "WEIGH", "WEIRD", "WHACK", "WHALE", "WHARF", "WHEAT", "WHEEL", "WHELP", "WHERE", "WHICH", "WHIFF", "WHILE", "WHINE", "WHINY", "WHIRL", "WHISK", "WHITE", "WHOLE", "WHOOP", "WHOSE", "WIDEN", "WIDTH", "WIELD", "WIMPY", "WINCE", "WINDY", "WISER", "WITCH", "WITTY", "WOKEN", "WOMAN", "WOMEN", "WOOER", "WORDY", "WORLD", "WORRY", "WORSE", "WORST", "WOULD", "WOUND", "WOVEN", "WRATH", "WREAK", "WRECK", "WRIST", "WRITE", "WRONG", "WROTE", "WRUNG", "YACHT", "YEARN", "YEAST", "YIELD", "YOUNG", "YOUTH", "ZEBRA", "ZESTY"];
        
        /// <summary>
        /// Actual correct answer, the value of KNUXY will always get overwritten.
        /// </summary>
        public static string answer = "KNUXY";

        /// <summary>
        /// List of chracters that have being guess that aren't in the answer.
        /// </summary>
        public List<char> missingCharacters = [];
        
        /// <summary>
        /// Which character we're changing.
        /// </summary>
        public int selectedCharacter;
        
        /// <summary>
        /// Which attempt at getting the answer we're on.
        /// </summary>
        public int answerAttempt;
        
        /// <summary>
        /// The object for the cursor.
        /// </summary>
        public GameObject cursor;
        
        /// <summary>
        /// The objects for the words.
        /// </summary>
        public GameObject[] words = new GameObject[6];

        /// <summary>
        /// A reference to the stage HUD.
        /// </summary>
        private FPHudMaster HUD;

        /// <summary>
        /// The set of characters displayed at the bottom.
        /// </summary>
        public TextMesh UntriedCharacters;

        // Variables for the intro animation.
        public int state = 0;
        public int typingIndex = 0;
        public float genericTimer = 0;

        void Start()
        {
            // Get the HUD and hide it.
            HUD = UnityEngine.Object.FindObjectOfType<FPHudMaster>();
            HUD?.state = 2;

            // Reset the player answer and missing characters.
            playerAnswer = "     ";
            missingCharacters = [];

            // Pick a random answer from our list.
            answer = answers[Plugin.rng.Next(answers.Length)];

            // Get the cursor object.
            cursor = transform.GetChild(0).gameObject;
            cursor.SetActive(false);

            // Get the word objects.
            words[0] = transform.GetChild(1).gameObject;
            words[1] = transform.GetChild(2).gameObject;
            words[2] = transform.GetChild(3).gameObject;
            words[3] = transform.GetChild(4).gameObject;
            words[4] = transform.GetChild(5).gameObject;
            words[5] = transform.GetChild(6).gameObject;

            // Get and clear the untried characters.
            UntriedCharacters = transform.GetChild(7).GetComponent<TextMesh>();
            UntriedCharacters.text = "";

            // Reset the state and position to the ones for the entrance animation.
            state = 0;
            transform.position = new(transform.position.x, 88, transform.position.z);

            // DEBUG: Log the correct answer.
            //Plugin.consoleLog.LogDebug($"Answer is: {answer}");
        }

        void Update()
        {
            // Check if we're in the first state for the intro animation.
            if (state == 0)
            {
                // Shift the Wordle Trap object down by eight pixels until it's reached 0.
                if (transform.position.y > 0)
                {
                    transform.position = new(transform.position.x, transform.position.y - (8 * FPStage.deltaTime), transform.position.z);
                }

                // Snap the Wordle Trap object to 0,0,0, reset the generic timer and move on to the typing state.
                else
                {
                    genericTimer = 0;
                    transform.position = Vector3.zero;
                    state = 1;
                }
                return;
            }

            // Check if we're in the typing state for the intro animation.
            if (state == 1)
            {
                // Increment the generic timer.
                genericTimer += FPStage.deltaTime;

                // Check the generic timer has reached 1.
                if (genericTimer >= 1)
                {
                    // Type out this index's character, or exit the intro animation if we've typed them all.
                    switch (typingIndex)
                    {
                        case 0: UntriedCharacters.text += "A "; break;
                        case 1: UntriedCharacters.text += "B "; break;
                        case 2: UntriedCharacters.text += "C "; break;
                        case 3: UntriedCharacters.text += "D "; break;
                        case 4: UntriedCharacters.text += "E "; break;
                        case 5: UntriedCharacters.text += "F "; break;
                        case 6: UntriedCharacters.text += "G "; break;
                        case 7: UntriedCharacters.text += "H "; break;
                        case 8: UntriedCharacters.text += "I "; break;
                        case 9: UntriedCharacters.text += "J "; break;
                        case 10: UntriedCharacters.text += "K "; break;
                        case 11: UntriedCharacters.text += "L "; break;
                        case 12: UntriedCharacters.text += "M "; break;
                        case 13: UntriedCharacters.text += "N "; break;
                        case 14: UntriedCharacters.text += "O "; break;
                        case 15: UntriedCharacters.text += "P "; break;
                        case 16: UntriedCharacters.text += "Q "; break;
                        case 17: UntriedCharacters.text += "R "; break;
                        case 18: UntriedCharacters.text += "S "; break;
                        case 19: UntriedCharacters.text += "T "; break;
                        case 20: UntriedCharacters.text += "U "; break;
                        case 21: UntriedCharacters.text += "V "; break;
                        case 22: UntriedCharacters.text += "W "; break;
                        case 23: UntriedCharacters.text += "X "; break;
                        case 24: UntriedCharacters.text += "Y "; break;
                        case 25: UntriedCharacters.text += "Z "; break;
                        default: state = 2; cursor.SetActive(true); break;
                    }

                    // Decrement the timer.
                    genericTimer -= FPStage.deltaTime;

                    // Increment the typing index.
                    typingIndex++;
                }

                return;
            }

            // Cycle between the characters using Left and Right.
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectedCharacter != 0)
                {
                    selectedCharacter--;
                    FPAudio.PlayMenuSfx(FPAudio.SFX_MOVE);
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectedCharacter < 4)
                {
                    selectedCharacter++;
                    FPAudio.PlayMenuSfx(FPAudio.SFX_MOVE);
                }
            }

            // Submit the answer.
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Don't do anything if all five characters aren't filled.
                if (playerAnswer.Contains(" "))
                    return;

                // Handle the answer being incorrect.
                if (playerAnswer != answer)
                {
                    // Play the invalid menu sound.
                    FPAudio.PlayMenuSfx(FPAudio.SFX_INVALID);

                    // Reset the selected character and cursor position, shifting the cursor down.
                    selectedCharacter = 0;
                    cursor.transform.position = new Vector3(224, cursor.transform.position.y - 48, cursor.transform.position.z);

                    // Copy the player's answer into a string builder.
                    System.Text.StringBuilder strBuilder = new(playerAnswer);

                    // Loop through each character.
                    for (int characterIndex = 0; characterIndex < 5; characterIndex++)
                    {
                        // Remove this character from the untried string.
                        UntriedCharacters.text = UntriedCharacters.text.Replace(playerAnswer[characterIndex], ' ');

                        // Check if this character in the player's answer doesn't match the one at the same position in the answer.
                        if (playerAnswer[characterIndex] != answer[characterIndex])
                        {
                            // Remove this character from the string.
                            strBuilder[characterIndex] = ' ';

                            // Colour the text and box red or yellow depending on whether or not this character is in the answer at all.
                            if (!answer.Contains(playerAnswer[characterIndex].ToString()))
                            {
                                words[answerAttempt].transform.GetChild(characterIndex).GetChild(0).GetComponent<TextMesh>().color = UnityEngine.Color.red;
                                words[answerAttempt].transform.GetChild(characterIndex).GetComponent<FPHudDigit>().SetDigitValue(1);

                                // Add this character to the missing character list.
                                missingCharacters.Add(playerAnswer[characterIndex]);
                            }
                            else
                            {
                                words[answerAttempt].transform.GetChild(characterIndex).GetChild(0).GetComponent<TextMesh>().color = UnityEngine.Color.yellow;
                                words[answerAttempt].transform.GetChild(characterIndex).GetComponent<FPHudDigit>().SetDigitValue(2);
                            }
                        }

                        // Colour the text and box green if the character is in the correct place.
                        else
                        {
                            words[answerAttempt].transform.GetChild(characterIndex).GetChild(0).GetComponent<TextMesh>().color = UnityEngine.Color.green;
                            words[answerAttempt].transform.GetChild(characterIndex).GetComponent<FPHudDigit>().SetDigitValue(3);
                        }
                    }

                    // Replace the answer with the string builder's value.
                    playerAnswer = strBuilder.ToString().ToUpper();

                    // Increment the attempt count and make the next line of characters visible.
                    if (answerAttempt != 5)
                    {
                        answerAttempt++;
                        words[answerAttempt].SetActive(true);
                    }
                    else
                    {
                        // Set the flag on the player to tell it we're dying because of the Wordle Trap.
                        FPPlayerPatcher.dyingFromWordleTrap = true;

                        // Play the "Bzzt! Wrong!" voice line.
                        FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("FP2_Zao_S10_06_aptrim"));

                        // Force run the player's crush action to kill them.
                        FPPlayerPatcher.player.Action_Crush();

                        // Remove the Wordle Trap flag.
                        Plugin.WordleTrap = false;

                        // Destroy this Wordle Trap.
                        GameObject.Destroy(this.gameObject);
                    }
                }
                else
                {
                    // Spawn 7 life petals, copied from the original code for breaking a life petal box.
                    for (float num = 22.5f; num <= 157.5f; num += 135f / (float)(7 - 1))
                    {
                        ItemPetal itemPetal = (ItemPetal)FPStage.CreateStageObject(ItemPetal.classID, FPPlayerPatcher.player.position.x, FPPlayerPatcher.player.position.y);
                        itemPetal.gameObject.layer = FPPlayerPatcher.player.gameObject.layer;
                        itemPetal.state = itemPetal.State_Released;
                        itemPetal.velocity.x = Mathf.Cos((FPPlayerPatcher.player.transform.eulerAngles.z + num) * ((float)Math.PI / 180f)) * 4f;
                        itemPetal.velocity.y = Mathf.Sin((FPPlayerPatcher.player.transform.eulerAngles.z + num) * ((float)Math.PI / 180f)) * 4f;
                    }

                    // Play the +5 sound.
                    FPAudio.PlaySfx(Plugin.apAssetBundle.LoadAsset<AudioClip>("GachaponWin"));

                    // Remove the Wordle Trap flag.
                    Plugin.WordleTrap = false;

                    // Bring back the HUD.
                    HUD?.state = 1;

                    // Destroy this Wordle Trap.
                    GameObject.Destroy(this.gameObject);
                }

                // Don't run the rest of the update function.
                return;
            }

            // Loop through the input string, which should in theory only have one character in it hopefully?
            foreach (char character in Input.inputString)
            {
                // Make sure this character is a letter.
                if (char.IsLetter(character))
                {
                    // Don't allow this letter to be typed if its already confirmed to not be in the answer.
                    if (missingCharacters.Contains(char.ToUpper(character)))
                    {
                        FPAudio.PlayMenuSfx(FPAudio.SFX_INVALID);
                        continue;
                    }

                    // Make a string builder and set the character at the selected position in it to the one the player input.
                    System.Text.StringBuilder strBuilder = new(playerAnswer);
                    strBuilder[selectedCharacter] = character;
                    playerAnswer = strBuilder.ToString().ToUpper();

                    // Play the tally sound.
                    FPAudio.PlayMenuSfx(FPAudio.SFX_TALLY);

                    // Move to the next character if we're not at the end of the string.
                    if (selectedCharacter < 4)
                        selectedCharacter++;
                }
            }

            // Move the cursor to the right X position for the selected character.
            switch (selectedCharacter)
            {
                case 0: cursor.transform.position = new Vector3(224, cursor.transform.position.y, cursor.transform.position.z); break;
                case 1: cursor.transform.position = new Vector3(272, cursor.transform.position.y, cursor.transform.position.z); break;
                case 2: cursor.transform.position = new Vector3(320, cursor.transform.position.y, cursor.transform.position.z); break;
                case 3: cursor.transform.position = new Vector3(368, cursor.transform.position.y, cursor.transform.position.z); break;
                case 4: cursor.transform.position = new Vector3(416, cursor.transform.position.y, cursor.transform.position.z); break;
            }

            // Update the character in each box.
            words[answerAttempt].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = playerAnswer[0].ToString();
            words[answerAttempt].transform.GetChild(1).GetChild(0).GetComponent<TextMesh>().text = playerAnswer[1].ToString();
            words[answerAttempt].transform.GetChild(2).GetChild(0).GetComponent<TextMesh>().text = playerAnswer[2].ToString();
            words[answerAttempt].transform.GetChild(3).GetChild(0).GetComponent<TextMesh>().text = playerAnswer[3].ToString();
            words[answerAttempt].transform.GetChild(4).GetChild(0).GetComponent<TextMesh>().text = playerAnswer[4].ToString();
        }
    }
}
