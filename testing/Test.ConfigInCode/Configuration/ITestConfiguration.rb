require 'rubygems'

class Configuration
    include Test::ConfigInCode::ITestConfiguration
    def ConstructorValue
        @constructorValue
    end
    
    def initialize(constructorValue)
        @constructorValue = constructorValue             
    end
end