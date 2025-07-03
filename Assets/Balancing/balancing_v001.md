# game balancing

## player stats
maxZeitsand: 100
zeitsandStartValue: 40
zeitsandRatePerSec: 2

## wave stats
timeBetweenWaves: 5
timeBetweenEnemies: 3
minSpawnDistance: 5
initialWaveBudget: 100
budgetIncrease: 10
maxWaveBudget: 1000
maxRoutesUsedByEnemies: 3

## tower stats
### STANDARD
range: 3
isSingleUse: True
useAmount: 1
buildingCost: 10
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 1
#### bullet stats
speed: 10
damage: 0,41
explosionRadius: 0
useFreeze: False
shootSelf: False
SlowMultiplier: 0
freezeDuration: 5

### DRONEBASE
range: 10
isSingleUse: True
useAmount: 1
buildingCost: 15
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 0,1

### DRONE
range: 10
isSingleUse: True
useAmount: 1
buildingCost: 1
isMoving: True
speed: 2
stopAndShootRange: 2
fireRate: 1
#### bullet stats
speed: 5
damage: 0,5
explosionRadius: 0
useFreeze: False
shootSelf: False
SlowMultiplier: 0
freezeDuration: 5

### MISSILE
range: 3
isSingleUse: True
useAmount: 1
buildingCost: 15
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 0,5
#### bullet stats
speed: 5
damage: 0,5
explosionRadius: 1,5
useFreeze: False
shootSelf: False
SlowMultiplier: 0
freezeDuration: 5

### LASER
range: 3
isSingleUse: True
useAmount: 1
buildingCost: 25
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 300
#### bullet stats
speed: 10
damage: 0,04
explosionRadius: 0
useFreeze: False
shootSelf: False
SlowMultiplier: 0
freezeDuration: 5

### FREEZE
range: 3
isSingleUse: True
useAmount: 1
buildingCost: 15
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 10
#### bullet stats
speed: 5
damage: 0
explosionRadius: 3
useFreeze: True
shootSelf: True
SlowMultiplier: 0,1
freezeDuration: 1

### DYNAMITE
range: 2
isSingleUse: True
useAmount: 1
buildingCost: 8
isMoving: False
speed: 30
stopAndShootRange: 5
fireRate: 1
#### bullet stats
speed: 5
damage: 1
explosionRadius: 3
useFreeze: False
shootSelf: True
SlowMultiplier: 0
freezeDuration: 5


### STANDARD
speed: 0,25
health: 2
isSplitter: False
splitAmount: 3
auraRadius: 0
auraEffectStrength: 2
auraDuration: 2
cost: 10
randomWeight: 100
damage: 5

### SPEED
speed: 1
health: 1
isSplitter: False
splitAmount: 0
auraRadius: 0
auraEffectStrength: 2
auraDuration: 2
cost: 10
randomWeight: 75
damage: 5

### SPLITTER
speed: 0,25
health: 1
isSplitter: True
splitAmount: 5
auraRadius: 0
auraEffectStrength: 2
auraDuration: 2
cost: 30
randomWeight: 50
damage: 30

### SUPPORT
speed: 0,25
health: 2
isSplitter: False
splitAmount: 0
auraRadius: 3
auraEffectStrength: 1,5
auraDuration: 5
cost: 20
randomWeight: 25
damage: 10

### TANK
speed: 0,125
health: 10
isSplitter: False
splitAmount: 0
auraRadius: 0
auraEffectStrength: 2
auraDuration: 2
cost: 30
randomWeight: 50
damage: 30

