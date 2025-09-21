import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useUserInfoQuery } from "../../features/authentication/api/account.api";

export default function RequireAuth() {
    const { data: user, isLoading } = useUserInfoQuery();
    const location = useLocation();

    if (isLoading) return;
    if (!user) return <Navigate to="/login" state={{ from: location }} />;
    const adminRoutes = ["/inventory", "/dashboard"];
    if (adminRoutes.includes(location.pathname) && !user.roles.includes("Admin")) return <Navigate to="/" replace />;
    return <Outlet />;
}
