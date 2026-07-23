import {Link} from 'react-router-dom';
const NavigationPanel = ({role}) => {
    
    const btnStyle = "text-zinc-400 text-[16px] hover:text-zinc-50 transition-colors duration-200";

    const renderRightPanel = () => {
        if(role==="Candidate")
        {
            return(
                <>
                    <Link to="/profile">
                        <button className={btnStyle}>Profile</button>
                    </Link>

                    <Link to="/auth">
                        <button className={btnStyle}>Logout</button>
                    </Link>
                </>
            );
        }

        if(role==="unauthorized"){
            return(
                <>
                    <Link to="/auth">
                        <button className={btnStyle}>Login</button>
                    </Link>
                </>
            );
        }

        if(role==="Recruiter" || role==="Admin"){
            return(
                <>
                    <Link to="/positions-management">
                            <button className={btnStyle}>Position Management</button>
                    </Link>
                    <Link to="/profile">
                        <button className={btnStyle}>Profile</button>
                    </Link>
                    <Link to="/attribute">
                            <button className={btnStyle}>Attribute Management</button>
                    </Link>
                    <Link to="/auth">
                        <button className={btnStyle}>Logout</button>
                    </Link>
                </>
            );
        }
    }


    return (
        <div className="w-full fixed min-h-20 border-b border-zinc-850 font-sans
         bg-zinc-950 flex flex-row items-center px-5  justify-between">

            <div className="flex items-center text-sm font-medium space-x-10 tracking-tight">
                <span className="text-zinc-50 text-[20px] font-semibold ">Recruitment system</span>
                <button className="text-zinc-400 text-[16px] hover:text-zinc-50 transition-colors duration-200">Main page</button>
                <button className="text-zinc-400 text-[16px] hover:text-zinc-50 transition-colors duration-200">Positions</button>
            </div>

            <div className="flex items-center justify-end text-sm font-medium space-x-10 pr-15">
                {renderRightPanel()}
            </div>

        </div>        
    );
}
export default NavigationPanel;