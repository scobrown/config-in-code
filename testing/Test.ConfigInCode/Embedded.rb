require 'rubygems'

class Embedded
    include Test::ConfigInCode::ITestConfiguration
    def ConstructorValue
        "#{@constructorValue} Embedded"
    end
    
    def initialize(constructorValue)
        @constructorValue = constructorValue             
    end
end